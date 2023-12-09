using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy01 : Enemy
{
    [Space, Header("Enemy 1")]
    [Header("Knockback"), SerializeField]
    private float knockbackScale;
    [SerializeField]
    private float knockbackRotation;

    [Header("Eating"), SerializeField]
    private float TimeEating;
    private float currentTimeEating;

    Rigidbody2D c_rb;

    private void Awake()
    {
        c_rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        currentTimeEating = 0;
    }
    void Start()
    {
        AssignMoveSpot();
        iaData.m_currentTarget = moveSpots[randomSpot];
        //moveSpot.position = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY));
        InitEnemy();
    }
    private void Update()
    {

        //CheckState();
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
                PerformDetection();
                PatrollingBehaviour(); 
                break;
            case EnemyStates.CHASING:
                PerformDetection();
                ChaseBehaviour(); 
                break;
            case EnemyStates.KNOCKBACK:
            // ... 
            case EnemyStates.EATING:
                EatingBehaviour();
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

        Vector2 direction = movementDirectionSolver.GetDirectionToMove(l_steeringBehaviours, iaData);

        c_rb2d.AddForce(direction * speed, ForceMode2D.Force);

        // ROTATION OF THE ENENMY WHILE FOLLOWING
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(Vector3.forward * angle);
        //Vector2 direction = movementDirectionSolver.GetDirectionToMove(l_steeringBehaviours, iaData);
        ////Vector2 direction = transform.position - moveSpots[randomSpot].position; 
        //transform.position = Vector2.MoveTowards(transform.position, moveSpot.position, 2 * Time.deltaTime);

        //if(Vector2.Distance(transform.position, moveSpot.position) < 0.2f)
        //{
        //    moveSpot.position = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY));
        //}
        //if (iaData.GetTargetsCount() != 0)
        //{
        //    currentState = EnemyStates.CHASING;
        //}

        //// ROTATION OF THE ENENMY WHILE FOLLOWING
        //float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //transform.rotation = Quaternion.Euler(Vector3.forward * angle);
    }
    override protected void ChaseBehaviour()
    {
        if(iaData.m_currentTarget == null)
        {
            currentState = EnemyStates.PATROLLING;
            return; 
        }
        Vector2 direction = movementDirectionSolver.GetDirectionToMove(l_steeringBehaviours, iaData);

        c_rb2d.AddForce(direction * speed, ForceMode2D.Force);

        // ROTATION OF THE ENENMY WHILE FOLLOWING
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(Vector3.forward * angle);

    }

    void Knockback(Vector2 collisionPoint)
    {
        ChangeState(EnemyStates.KNOCKBACK);
        Vector2 direction = (Vector2)transform.position - collisionPoint;
        direction.Normalize();

        c_rb.AddForce(direction * knockbackScale, ForceMode2D.Impulse);
        c_rb.AddTorque(Random.Range(-knockbackRotation, knockbackRotation), ForceMode2D.Impulse);
        ChangeState(EnemyStates.EATING);
    }

    protected void EatingBehaviour()
    {
        currentTimeEating += Time.deltaTime;

        if(currentTimeEating >= TimeEating)
        {
            currentTimeEating = 0;
            ChangeState(EnemyStates.CHASING);
        }
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
    //    CheckIsFollowing();

    //    if (currentState == EnemyStates.KNOCKBACK || currentState == EnemyStates.EATING)
    //        return;

    //    if (isFollowing && currentState != EnemyStates.EATING)
    //    {
    //        ChangeState(EnemyStates.CHASING);
    //    }
    //    else
    //    {
    //        ChangeState(EnemyStates.PATROLLING);
    //    }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Player"))
        {
            Knockback(collision.GetContact(0).point);
            currentHealth += 20;
        }
    }

}
