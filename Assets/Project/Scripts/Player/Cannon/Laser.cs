using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private int damage;
    [SerializeField]
    private float maxDistance;

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
            DisableObject();
        }
    }

    public float GetBulletDamage()
    {
        return damage * PowerUpManager.Instance.Damage;
    }
    private void DisableObject()
    {
        c_rb.velocity = Vector2.zero;
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Map") || collision.CompareTag("BreakableWall"))
        {
            AudioManager._instance.Play2dOneShotSound(mapHit, "Laser");
            DisableObject();
        }
        if (collision.CompareTag("Enemy"))
        {
            AudioManager._instance.Play2dOneShotSound(enemyHit, "Laser");
            DisableObject();
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, autoAimRadius);
    }
}
