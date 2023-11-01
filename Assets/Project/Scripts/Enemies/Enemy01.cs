using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy01 : Enemy, IHealth //enemigo que te persigue i ja esta
{
    public enum EnemyStates { PATROLLING, CHASING, KNOCKBACK}
    public EnemyStates currentState = EnemyStates.PATROLLING;

    void Start()
    {
        InitEnemy();
    }
    private void Update()
    {
        CheckState();
    }
    private void FixedUpdate()
    {
        Behaviour(); 
    }

    // ENEMY
    override protected void Behaviour()
    {
        switch (currentState)
        {
            case EnemyStates.PATROLLING:
                PatrollingBehaviour(); 
                break;
            case EnemyStates.CHASING:
                ChaseBehaviour(); 
                break;
            case EnemyStates.KNOCKBACK:
                // ... 
                break; 
            default:
                break;
        }
    }
    override protected void PatrollingBehaviour()
    {
        // ...
    }
    override protected void ChaseBehaviour()
    {
        Vector2 direction = movementDirectionSolver.GetDirectionToMove(l_steeringBehaviours, iaData);

        c_rb2d.AddForce(direction * speed, ForceMode2D.Force);

        // ROTATION OF THE ENENMY WHILE FOLLOWING
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(Vector3.forward * angle);

    }
    protected void ChangeState(EnemyStates nextState)
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
    override protected void CheckState()
    {
        CheckIsFollowing(); 

        if (currentState == EnemyStates.KNOCKBACK)
            return; 

        if(isFollowing)
        {
            ChangeState(EnemyStates.CHASING);
        }
        else
        {
            ChangeState(EnemyStates.PATROLLING);
        }
    }
    // HEALTH
    public void GetHit(int amount)
    {
        currentHealth -= amount;

        if (currentHealth < 0)
        {
            isDead = true;
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Laser"))
        {
            GetHit(20); 
        }
    }
}
