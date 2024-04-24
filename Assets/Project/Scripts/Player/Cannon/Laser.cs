using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float bulletSpeed;
    [SerializeField]
    private float bossBulletSpeed;
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
    private Rigidbody2D rb2d;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
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
        float currentSpeed = BossManager.Instance.onBoss ? bossBulletSpeed : bulletSpeed;
        rb2d.velocity = transform.up * currentSpeed;
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
            rb2d.velocity = (autoAimTarget.transform.position - transform.position).normalized * bulletSpeed * TimeManager.Instance.timeParameter;
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
        rb2d.velocity = Vector2.zero;
        Destroy(gameObject);
    }

    private void LaserPause()
    {
        rb2d.velocity = Vector2.zero;
        rb2d.angularVelocity = 0.0f;
    }

    private void LaserResume()
    {
        rb2d.velocity = transform.up * bulletSpeed;
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
