using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EelObstacle : MonoBehaviour
{
    [SerializeField]
    private GameObject attackPoint;
    [SerializeField]
    private GameObject spawnPoint; 

    private Rigidbody2D rb2d;

    private Transform currentPoint; 

    [SerializeField]
    private float speed;

    [SerializeField]
    private bool drawGizmos = true; 

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        currentPoint = spawnPoint.transform; 
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            transform.GetChild(0).position = Vector2.MoveTowards(transform.GetChild(0).position, transform.GetChild(1).position, speed * Time.deltaTime);
        }
    }

    private void Update()
    {
        if(Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == spawnPoint.transform)
        {
            currentPoint = attackPoint.transform;
        }
        if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == attackPoint.transform)
        {
            currentPoint = spawnPoint.transform;
        }
    }

    private void FixedUpdate()
    {
        Vector2 point = currentPoint.position - transform.position;
        if(currentPoint == spawnPoint.transform)
        {
            rb2d.velocity = new Vector2(speed, 0); 
        }
        else
        {
            rb2d.velocity = new Vector2(-speed, 0); 
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            transform.GetChild(0).position = Vector2.MoveTowards(transform.GetChild(0).position, transform.position, speed * Time.deltaTime);
        }
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos)
            return; 

        Gizmos.DrawWireSphere(spawnPoint.transform.position, 0.5f); 
        Gizmos.DrawWireSphere(attackPoint.transform.position, 0.5f);
        Gizmos.DrawLine(spawnPoint.transform.position, attackPoint.transform.position); 
    }
}
