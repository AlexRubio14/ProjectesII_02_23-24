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

    private void InstantiateWallLaserHit()
    { 

        RaycastHit2D upHit = Physics2D.Raycast(transform.position, Vector2.up, 100, mapLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(transform.position, Vector2.right, 100, mapLayer);
        RaycastHit2D leftHit = Physics2D.Raycast(transform.position, Vector2.left, 100, mapLayer);
        RaycastHit2D downHit = Physics2D.Raycast(transform.position, Vector2.down, 100, mapLayer);


        Vector2 spawnPos = upHit.point;
        float currDistance = upHit.distance;
        Vector2 targetRotation = Vector2.down;
        
        if (rightHit.distance < currDistance)
        {
            spawnPos = rightHit.point;
            currDistance = rightHit.distance;
            targetRotation = Vector2.left;
        }
        
        if (leftHit.distance < currDistance)
        {
            spawnPos = leftHit.point;
            currDistance = leftHit.distance;
            targetRotation = Vector2.right;
        }

        if (downHit.distance < currDistance)
        {
            spawnPos = downHit.point;
            targetRotation = Vector2.up;
        }

        GameObject laserCollision = Instantiate(collisionLaserPrefab, spawnPos, Quaternion.identity);

        laserCollision.transform.up = targetRotation;

    }

    private void InstantiateBreakableWallLaserHit(Vector2 _hitPos)
    {
        GameObject laserCollision = Instantiate(collisionLaserPrefab, _hitPos, Quaternion.identity);

        Vector2 direction = ((Vector2)transform.position - _hitPos).normalized;
        float upDot = Vector2.Dot(Vector2.up, direction);
        float rightDot = Vector2.Dot(Vector2.right, direction);
        float leftDot = Vector2.Dot(Vector2.left, direction);
        float downDot = Vector2.Dot(Vector2.down, direction);

        float currDot = upDot;
        Vector2 targetRotation = Vector2.up;

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

    private void InstantiateEnemyLaserHit(Vector2 _hitPos, Transform _enemyTransform)
    {
        GameObject laserCollision = Instantiate(collisionLaserPrefab, _hitPos, Quaternion.identity);

        Vector2 direction = ((Vector2)transform.position - _hitPos).normalized;
        float upDot = Vector2.Dot(_enemyTransform.up, direction);
        float rightDot = Vector2.Dot(_enemyTransform.right, direction);
        float leftDot = Vector2.Dot(-_enemyTransform.right, direction);
        float downDot = Vector2.Dot(-_enemyTransform.up, direction);

        float currDot = upDot;
        Vector2 targetRotation = _enemyTransform.up;

        if (rightDot > currDot)
        {
            currDot = rightDot;
            targetRotation = _enemyTransform.right;
        }

        if (leftDot > currDot)
        {
            currDot = leftDot;
            targetRotation = -_enemyTransform.right;
        }

        if (downDot > currDot)
        {
            targetRotation = -_enemyTransform.up;
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
        if (collision.CompareTag("Map"))
        {
            InstantiateWallLaserHit();
            source.PlayOneShot(mapHit);
            DestroyBullet();
        }
        else if(collision.CompareTag("BreakableWall"))
        {
            InstantiateBreakableWallLaserHit(collision.gameObject.GetComponent<Collider2D>().ClosestPoint(transform.position));
            source.PlayOneShot(mapHit);
            DestroyBullet();
        }
        else if (collision.CompareTag("Enemy"))
        {
            InstantiateEnemyLaserHit(collision.gameObject.GetComponent<Collider2D>().ClosestPoint(transform.position), collision.transform);
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
