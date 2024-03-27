using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy02 : Enemy
{
    [Space, Header("--- ENEMY 02"), SerializeField]
    private float timeFollowing = 5.0f;
    [SerializeField]
    private GameObject c_explosionParticles;

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

        FlipSprite();
    }
    override protected void PatrollingBehaviour()
    {
        if (iaData.m_currentTarget == null)
        {
            if (moveSpots.Length > 0)
            { 
                AssignMoveSpot();
                iaData.m_currentTarget = moveSpots[randomSpot];
            }
            else
            {
                iaData.m_currentTarget = transform;
            }
        }
        MoveToTarget();
    }
    override protected void ChaseBehaviour()
    {
        if (iaData.m_currentTarget == null)
        {
            ChangeState(EnemyStates.PATROLLING);
            return;
        }
        MoveToTarget();

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
                c_rb2d.velocity = Vector2.zero;
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
                Invoke("Die", timeFollowing);
                break;
            case EnemyStates.KNOCKBACK:
                break;
            default:
                break;
        }
    }

    override public void Die()
    {
        AudioManager._instance.Play2dOneShotSound(deathClip, "Enemy", 1, 0.9f, 1.1f);
        Instantiate(c_explosionParticles, transform.position, Quaternion.identity);
        base.Die();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Player"))
            Die();
        if (collision.collider.CompareTag(BULLET_TAG))
        {
            float bulletDamage = collision.collider.GetComponent<Laser>().GetBulletDamage();
            GetHit(bulletDamage);
            ChangeState(EnemyStates.KNOCKBACK);
            StartKnockback(collision.transform.position, knockbackForce);
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        
    }
}
