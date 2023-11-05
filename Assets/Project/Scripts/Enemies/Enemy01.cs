using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy01 : Enemy
{
    

    //[Space, Header("Enemy 1")]
    

    private void Awake()
    {
        currentHealth = maxHealth;
    }
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
            case EnemyStates.EATING:
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
    override protected void ChangeState(EnemyStates nextState)
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

    

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Player"))
        {
            StartEating();
        }
    }

}
