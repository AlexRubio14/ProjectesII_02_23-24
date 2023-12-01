using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EelObstacle : MonoBehaviour
{
    [SerializeField]
    private GameObject sprite;
    [SerializeField]
    private GameObject spawnPoint;

    [SerializeField]
    private float attackDelay = 1.0f;
    private bool isAttacking = false;

    [SerializeField]
    private float speed = 7.0f;

    [SerializeField]
    private bool drawGizmos = true;

    private void Start()
    {
        sprite.transform.position = spawnPoint.transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isAttacking = true;
        }
    }

    private void Update()
    {
        if (isAttacking)
        {
            sprite.transform.position = Vector2.MoveTowards(sprite.transform.position, transform.position, speed * Time.deltaTime);
        }
        else
        {
            sprite.transform.position = Vector2.MoveTowards(sprite.transform.position, spawnPoint.transform.position, speed * Time.deltaTime);
        }

        if (Vector2.Distance(sprite.transform.position, transform.position) < 0.5f)
        {
            Invoke("SetAttackFalse", attackDelay);
        }
    }

    private void SetAttackFalse()
    {
        isAttacking = false;
    }

    private void OnDrawGizmos()
    {
        if (!drawGizmos)
            return;

        Gizmos.DrawWireSphere(spawnPoint.transform.position, 0.5f);
        Gizmos.DrawWireSphere(this.transform.position, 0.5f);
        Gizmos.DrawLine(spawnPoint.transform.position, this.transform.position);
    }
}
