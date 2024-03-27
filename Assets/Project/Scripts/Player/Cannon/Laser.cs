using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private int damage;
    [SerializeField]
    private float maxDistance;

    [Space, Header("Collision Particles"), SerializeField]
    private GameObject collisionLaserPrefab;
    [SerializeField]
    private LayerMask mapLayer;

    [Space, Header("Auto Aim"), SerializeField]
    private float autoAimRadius;
    [SerializeField]
    private LayerMask enemyLayer;
    private GameObject autoAimTarget;

    [Space, Header("Audio"), SerializeField]
    private AudioClip enemyHit;
    [SerializeField]
    private AudioClip mapHit;

    private Vector2 startPosition;
    private float currentDistance;
    private Rigidbody2D c_rb;

    private void Awake()
    {
        c_rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        TimeManager.Instance.pauseAction += LaserPause;
        TimeManager.Instance.resumeAction += LaserResume;
    }

    private void OnDestroy()
    {
        TimeManager.Instance.pauseAction -= LaserPause;
        TimeManager.Instance.resumeAction -= LaserResume;
    }

    private void Start()
    {
        startPosition = transform.position;
        c_rb.velocity = transform.up * speed;
    }

    private void Update()
    {

        AutoAim();

        CheckDestroyBullet();
    }

    private void AutoAim()
    {
        if (!autoAimTarget)
        {
            RaycastHit2D hit = Physics2D.CircleCast(transform.position, autoAimRadius, Vector2.zero, 0, enemyLayer);

            if (hit && hit.rigidbody)
            {
                autoAimTarget = hit.rigidbody.gameObject;
            }
        }
        else if (Vector2.Dot ((autoAimTarget.transform.position - transform.position).normalized, transform.up) > 0.2f)
        {
            c_rb.velocity = (autoAimTarget.transform.position - transform.position).normalized * speed * TimeManager.Instance.timeParameter;
        }
    }

    private void CheckDestroyBullet()
    {
        currentDistance = Vector2.Distance(transform.position, startPosition);

        if (currentDistance >= maxDistance)
        {
            DestroyBullet();
        }
    }

    public float GetBulletDamage()
    {
        return damage * PowerUpManager.Instance.Damage;
    }

    private void InstantiateLaserHit(Collision2D _hitColl)
    {
        Vector2 hitPos = _hitColl.contacts[0].point;
        GameObject laserCollision = Instantiate(collisionLaserPrefab, hitPos, Quaternion.identity);

        laserCollision.transform.up = _hitColl.contacts[0].normal;
    }

    private void DestroyBullet()
    {
        //Comprobar la rotacion de donde deberia chocar
        //Crear el cosos que choca
        c_rb.velocity = Vector2.zero;
        Destroy(gameObject);
    }

    private void LaserPause()
    {
        c_rb.velocity = Vector2.zero;
        c_rb.angularVelocity = 0.0f;
    }

    private void LaserResume()
    {
        c_rb.velocity = transform.up * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Map") || collision.collider.CompareTag("BreakableWall"))
        {
            InstantiateLaserHit(collision);
            AudioManager.instance.Play3dOneShotSound(mapHit, "Laser", 5, collision.transform.position);
            DestroyBullet();
        }
        else if (collision.collider.CompareTag("Enemy") || collision.collider.CompareTag("Boss"))
        {
            InstantiateLaserHit(collision);
            AudioManager.instance.Play3dOneShotSound(enemyHit, "Laser", 5, collision.transform.position);
            DestroyBullet();
        }
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, autoAimRadius);
    }
}
