using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float speed;
    [SerializeField]
    private int damage;
    [SerializeField]
    private float maxDistance;

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
        currentDistance = Vector2.Distance(transform.position, startPosition);

        if(currentDistance >= maxDistance)
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

    
}
