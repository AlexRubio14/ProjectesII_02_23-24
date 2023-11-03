using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy01 : Enemy
{
    public enum EnemyStates { PATROLLING, CHASING, KNOCKBACK}

    [Space, Header("Enemy 1")]
    public EnemyStates currentState = EnemyStates.PATROLLING;

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BulletCollision(collision, 40);
    }
}
