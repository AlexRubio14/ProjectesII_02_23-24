using System.Collections;
using System.Collections.Generic;
using UnityEditor.Timeline;
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

    private Vector2 startPosition;
    private float currentDistance;
    private Rigidbody2D c_rb;

    private void Awake()
    {
        c_rb = GetComponent<Rigidbody2D>();
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
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, autoAimRadius, Vector2.zero, 0, enemyLayer);

        if (hit)
        {
            c_rb.velocity = (transform.up + (hit.transform.position - transform.position)).normalized;
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
            DisableObject();
        }
        if (collision.CompareTag("Enemy"))
        {
            DisableObject();
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, autoAimRadius);
    }
}
