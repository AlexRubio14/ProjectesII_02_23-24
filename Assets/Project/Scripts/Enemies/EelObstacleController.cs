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
    private GameObject player; 

    [SerializeField]
    private float attackDelay = 1.0f;
    private bool isWatching = false;

    private Vector2 attackPoint; 
    private bool isAttacking = false;
    private bool isInSpawnPoint = false;

    [SerializeField]
    private float speed = 7.0f;

    [SerializeField]
    private bool drawGizmos = true;

    private void Start()
    {
        sprite.transform.position = spawnPoint.transform.position;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if(!isAttacking && isInSpawnPoint)
            {
                Invoke("SetWatchFalse", attackDelay);
                isWatching = true;
            }
        }
    }

    private void Update()
    {
        if (isWatching)
        {
            Vector2 direction = sprite.transform.position - player.transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            sprite.transform.rotation = Quaternion.Euler(Vector3.forward * angle);
        }
        if(isAttacking)
        {
            sprite.transform.position = Vector2.MoveTowards(sprite.transform.position, attackPoint, speed * Time.deltaTime);
            if (Vector2.Distance(sprite.transform.position, attackPoint) < 0.5f)
            {
                isAttacking = false; 
            }
        }
        else
        {
            sprite.transform.position = Vector2.MoveTowards(sprite.transform.position, spawnPoint.transform.position, speed * Time.deltaTime);
            if (Vector2.Distance(sprite.transform.position, spawnPoint.transform.position) < 0.5f)
            {
                isInSpawnPoint = true;
            }
        }
        
    }

    private void SetWatchFalse()
    {
        isWatching = false;
        attackPoint = player.transform.position; 
        isAttacking = true;
        isInSpawnPoint = false; 
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
