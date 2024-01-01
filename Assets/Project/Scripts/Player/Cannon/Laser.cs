using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private int damage;
    [SerializeField]
    private float maxDistance;

    [Space, SerializeField]
    private GameObject collisionLaserPrefab;
    [Space, Header("Auto Aim"), SerializeField]
    private float autoAimRadius;
    [SerializeField]
    private LayerMask enemyLayer;
    private GameObject autoAimTarget;

    [Space, Header("Audio"), SerializeField]
    private AudioClip enemyHit;
    [SerializeField]
    private AudioClip mapHit;
    private AudioSource source;

    private Vector2 startPosition;
    private float currentDistance;
    private Rigidbody2D c_rb;

    private void Awake()
    {
        c_rb = GetComponent<Rigidbody2D>();
        source = GetComponent<AudioSource>();
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

            if (hit)
            {
                autoAimTarget = hit.rigidbody.gameObject;
            }
        }
        else
        {
            c_rb.velocity = (autoAimTarget.transform.position - transform.position).normalized * speed;
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

    private void InstantiateLaserHit(Vector2 _spawnPos)
    {
        GameObject laserCollision = Instantiate(collisionLaserPrefab, _spawnPos, Quaternion.identity);
        Vector2 targetRotation = Vector2.zero;

        Vector2 direction = ((Vector2)transform.position - _spawnPos).normalized;
        float upDot = Vector2.Dot(Vector2.up, direction);
        float rightDot = Vector2.Dot(Vector2.right, direction);
        float leftDot = Vector2.Dot(Vector2.left, direction);
        float downDot = Vector2.Dot(Vector2.down, direction);

        float currDot = upDot;
        targetRotation = Vector2.up;
        
        if (rightDot > currDot)
        {
            currDot = rightDot;
            targetRotation = Vector2.right;
        }
        
        if (leftDot > currDot)
        {
            currDot = leftDot;
            targetRotation = Vector2.left;
        }

        if (downDot > currDot)
        {
            targetRotation = Vector2.down;
        }


        laserCollision.transform.up = targetRotation;
    }

    private void DestroyBullet()
    {
        //Comprobar la rotacion de donde deberia chocar
        //Crear el cosos que choca
        c_rb.velocity = Vector2.zero;
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Map") || collision.CompareTag("BreakableWall"))
        {
            source.PlayOneShot(mapHit);
            InstantiateLaserHit(collision.gameObject.GetComponent<Collider2D>().ClosestPoint(transform.position));


            DestroyBullet();
        }
        if (collision.CompareTag("Enemy"))
        {
            source.PlayOneShot(enemyHit);
            DestroyBullet();
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, autoAimRadius);
    }
}
