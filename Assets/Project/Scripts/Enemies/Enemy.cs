using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class Enemy : EnemyIA, IHealth
{
    public enum EnemyStates { PATROLLING, CHASING, KNOCKBACK, EXTRA }
    public EnemyStates currentState { get; protected set; }

    protected Rigidbody2D c_rb2d;

    [Space, Header("Base Enemy"), SerializeField]
    protected float maxHealth;
    protected float currentHealth;
    [SerializeField]
    protected float speed;

    [SerializeField]
    public float damage { get; protected set; }
    protected string BULLET_TAG = "Bullet";

    [Header("Patrol"), SerializeField]
    protected Transform[] moveSpots;
    protected int randomSpot;

    [Header("Knockback"), SerializeField]
    protected float knockbackScale;
    [SerializeField]
    protected float knockbackRotation;
    [SerializeField]
    private float knockbackDuration;
    private float knockbackWaited;

    [Header("Drop"), SerializeField]
    protected ItemObject c_currentDrop;
    [SerializeField]
    protected GameObject c_pickableItemPrefab;
    [SerializeField]
    protected float maxThrowSpeed;

    public void InitEnemy()
    {
        c_rb2d = GetComponent<Rigidbody2D>();
        ChangeState(EnemyStates.PATROLLING);
        currentHealth = maxHealth;
    }


    #region Behaviours Functions
    protected abstract void Behaviour();
    protected abstract void PatrollingBehaviour();
    protected abstract void ChaseBehaviour();
    protected void KnockbackBehaviour()
    {
        knockbackWaited += Time.fixedDeltaTime; 
        if(knockbackWaited > knockbackDuration)
            ChangeState(EnemyStates.CHASING);
    }

    protected void StartKnockback(Vector2 collisionPoint)
    {
        ChangeState(EnemyStates.KNOCKBACK);
        knockbackWaited = 0.0f; 

        Vector2 direction = (Vector2)transform.position - collisionPoint;
        direction.Normalize();

        c_rb2d.AddForce(direction * knockbackScale, ForceMode2D.Impulse);
        c_rb2d.AddTorque(Random.Range(-knockbackRotation, knockbackRotation), ForceMode2D.Impulse);
    }

    protected void MoveToTarget()
    {
        Vector2 direction = movementDirectionSolver.GetDirectionToMove(l_steeringBehaviours, iaData);

        c_rb2d.AddForce(direction * speed, ForceMode2D.Force);

        // ROTATION OF THE ENENMY WHILE FOLLOWING
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(Vector3.forward * angle);
    }

    protected void AssignMoveSpot()
    {
        int randomValue = Random.Range(0, moveSpots.Length);
        if (randomValue == randomSpot)
        {
            AssignMoveSpot();
            return;
        }
        randomSpot = randomValue;
    }
    #endregion

    public abstract void ChangeState(EnemyStates nextState);

    #region Damage Functions
    public void GetHit(float _damageAmount)
    {
        currentHealth -= _damageAmount;

        if (currentHealth <= 0)
            Die();
    }
    virtual protected void Die()
    {
        if (c_currentDrop)
            DropItem();

        Destroy(gameObject);
    }
    protected void DropItem()
    {
        PickableItemController currItem = Instantiate(c_pickableItemPrefab, transform.position, Quaternion.identity).GetComponent<PickableItemController>();

        currItem.c_currentItem = c_currentDrop;

        float randomX = Random.Range(-1, 2);
        float randomY = Random.Range(-1, 2);
        Vector2 randomDir = new Vector2(randomX, randomY);

        randomDir.Normalize();

        float throwSpeed = Random.Range(0, maxThrowSpeed);
        currItem.ImpulseItem(randomDir, throwSpeed);
        currItem.transform.up = randomDir;
    }
    #endregion
}
