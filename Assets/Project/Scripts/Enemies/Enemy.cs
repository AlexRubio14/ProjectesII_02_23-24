using UnityEngine;
public abstract class Enemy : EnemyIA, IHealth
{
    public enum EnemyStates { PATROLLING, CHASING, KNOCKBACK, EXTRA }
    public EnemyStates currentState { get; protected set; }

    protected Rigidbody2D c_rb2d;

    [field: Space, Header("--- BASE ENEMY"), SerializeField]
    public float maxHealth { get; protected set; }
    public float currentHealth { get; protected set; }
    [SerializeField]
    protected float speed;

    [field: SerializeField]
    public float damage { get; protected set; }
    protected string BULLET_TAG = "Bullet";

    [Header("--- PATROL"), SerializeField]
    protected Transform[] moveSpots;
    protected int randomSpot;

    [Header("--- KNOCKBACK"), SerializeField]
    protected float knockbackForce;
    [SerializeField]
    protected float knockbackRotation;
    [SerializeField]
    private float knockbackDuration;
    private float knockbackWaited;

    [Header("--- DROP"), SerializeField]
    protected ItemObject c_currentDrop;
    [SerializeField]
    protected GameObject c_pickableItemPrefab;
    [SerializeField]
    protected float maxThrowSpeed;

    [Header("--- DEATH"), SerializeField]
    protected AudioClip deathClip;
    private string enemyAudioSourceName = "Enemy";

    //DEBUG
    [Space, SerializeField]
    private bool showGizmos = true;

    protected SpriteRenderer spriteR;

    public void InitEnemy()
    {
        c_rb2d = GetComponent<Rigidbody2D>();
        spriteR = GetComponentInChildren<SpriteRenderer>();
        ChangeState(EnemyStates.PATROLLING);
        currentHealth = maxHealth;
    }

    protected void OnEnable()
    {
        TimeManager.Instance.pauseAction += EnemyPause;
    }

    protected void OnDisable()
    {
        TimeManager.Instance.pauseAction -= EnemyPause;

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

    protected void StartKnockback(Vector2 collisionPoint, float force)
    {
        knockbackWaited = 0.0f; 

        Vector2 direction = (Vector2)transform.position - collisionPoint;
        direction.Normalize();

        c_rb2d.AddForce(direction * force, ForceMode2D.Impulse);
        c_rb2d.AddTorque(Random.Range(-knockbackRotation, knockbackRotation), ForceMode2D.Impulse);
    }

    protected void MoveToTarget()
    {
        Vector2 direction = movementDirectionSolver.GetDirectionToMove(l_steeringBehaviours, iaData);

        c_rb2d.AddForce(direction * speed * TimeManager.Instance.timeParameter, ForceMode2D.Force);

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

    protected void FlipSprite()
    {
        float rightDot = Vector2.Dot(Vector2.right, transform.right);
        float leftDot = Vector2.Dot(Vector2.left, transform.right);
        if (rightDot >= leftDot)
        {
            spriteR.flipY = false;
        }
        else
        {
            spriteR.flipY = true;
        }
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
    virtual public void Die()
    {
        if (c_currentDrop)
            DropItem();

        AudioManager.instance.Play2dOneShotSound(deathClip, enemyAudioSourceName);

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

        currItem.GetComponentInChildren<UnityEngine.Rendering.Universal.Light2D>().color = c_currentDrop.EffectsColor;
    }
    #endregion

    private void EnemyPause()
    {
        c_rb2d.velocity = Vector2.zero;
        c_rb2d.angularVelocity = 0.0f;
    }


    private void OnDrawGizmosSelected()
    {
        if (showGizmos == false)
            return;

        foreach (Transform spot in moveSpots)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(spot.position, 0.2f);
        }
    }
}
