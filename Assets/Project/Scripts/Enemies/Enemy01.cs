using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy01 : Enemy
{
    void Awake()
    {
        InitEnemy();
    }

    private void FixedUpdate()
    {
        Behaviour(); 
    }

    override protected void Behaviour()
    {
        switch (currentState)
        {
            case EnemyStates.PATROLLING:
                PerformDetection();
                PatrollingBehaviour(); 
                break;
            case EnemyStates.CHASING:
                PerformDetection();
                ChaseBehaviour(); 
                break;
            case EnemyStates.KNOCKBACK:
                KnockbackBehaviour(); 
                break; 
            default:
                break;
        }
    }

    override protected void PatrollingBehaviour()
    {
        if(iaData.m_currentTarget == null)
        {
            AssignMoveSpot();
            iaData.m_currentTarget = moveSpots[randomSpot]; 
        }
        MoveToTarget(); 
    }
    override protected void ChaseBehaviour()
    {
        if(iaData.m_currentTarget == null)
        {
            ChangeState(EnemyStates.PATROLLING);
            return; 
        }
        MoveToTarget(); 
    }

    protected void StopEating()
    {
        currentHealth += 20;
        ChangeState(EnemyStates.CHASING);
        
    }
    private void StartEating(Vector2 collisionPoint)
    {
        StartKnockback(collisionPoint);
        ChangeState(EnemyStates.EXTRA);
        Invoke("StopEating", 0); 
    }


    override public void ChangeState(EnemyStates nextState)
    {
        if (currentState == nextState)
            return;

        // EXIT ACTUAL STATE
        switch (currentState)
        {
            case EnemyStates.PATROLLING:
                break;
            case EnemyStates.CHASING:
                break;
            case EnemyStates.KNOCKBACK:
                break;
            default:
                break;
        }
        currentState = nextState;

        // INIT NEW STATE
        switch (currentState)
        {
            case EnemyStates.PATROLLING:
                break;
            case EnemyStates.CHASING:
                break;
            case EnemyStates.KNOCKBACK:
                break;
            default:
                break;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Player"))
        {
            StartEating(collision.contacts[0].point); 
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(BULLET_TAG) && currentState != EnemyStates.EXTRA)
        {
            float bulletDamage = collision.GetComponent<Laser>().GetBulletDamage();
            GetHit(bulletDamage);
            StartKnockback(collision.transform.position); 
        }
    }
}
