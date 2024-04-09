using UnityEngine;
public abstract class Enemy : EnemyIA, IHealth
{
    public enum EnemyStates { PATROLLING, CHASING, KNOCKBACK, EXTRA }
    public EnemyStates currentState { get; protected set; }

    protected Rigidbody2D rb2d;

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
    protected ItemObject currentDrop;
    [SerializeField]
    protected Vector2 minMaxItemDrop; 
    [SerializeField]
    protected GameObject pickableItemPrefab;
    [SerializeField]
    private GameObject bubblePrefab;
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
        rb2d = GetComponent<Rigidbody2D>();
        spriteR = GetComponentInChildren<SpriteRenderer>();
        ChangeState(EnemyStates.PATROLLING);
        currentHealth = maxHealth;
    }

    protected void OnEnable()
    {
        TimeManager.Instance.pauseAction += EnemyPause;
        TimeManager.Instance.resumeAction += EnemyResume;
    }

    protected void OnDisable()
    {
        TimeManager.Instance.pauseAction -= EnemyPause;
        TimeManager.Instance.resumeAction -= EnemyResume;
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

        rb2d.AddForce(direction * force, ForceMode2D.Impulse);
        rb2d.AddTorque(Random.Range(-knockbackRotation, knockbackRotation), ForceMode2D.Impulse);
    }

    protected void MoveToTarget()
    {
        Vector2 direction = movementDirectionSolver.GetDirectionToMove(steeringBehaviours, iaData);

        rb2d.AddForce(direction * speed * TimeManager.Instance.timeParameter, ForceMode2D.Force);

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
        if (currentDrop)
        {
            DropItems();
            DropBubble();
        }

        AudioManager.instance.Play2dOneShotSound(deathClip, enemyAudioSourceName);

        Destroy(gameObject);
    }
    protected void DropItems()
    {
        int randomItems = Random.Range((int)minMaxItemDrop.x, (int)minMaxItemDrop.y + 1);

        for (int i = 0; i < randomItems; i++)
        {
            PickableItemController currItem = Instantiate(pickableItemPrefab, transform.position, Quaternion.identity).GetComponent<PickableItemController>();

            currItem.InitializeItem(currentDrop);

            float randomX = Random.Range(-1, 2);
            float randomY = Random.Range(-1, 2);
            Vector2 randomDir = new Vector2(randomX, randomY);

            randomDir.Normalize();

            float throwSpeed = Random.Range(0, maxThrowSpeed);
            currItem.ImpulseItem(randomDir, throwSpeed);
            currItem.transform.up = randomDir;

            currItem.GetComponentInChildren<UnityEngine.Rendering.Universal.Light2D>().color = currentDrop.EffectsColor;
        }  
    }
    protected void DropBubble()
    {
        FuelBubbleController bubble = Instantiate(bubblePrefab, transform.position, Quaternion.Euler(0, 0, 0)).GetComponent<FuelBubbleController>();

        float randomX = Random.Range(-1, 2);
        float randomY = Random.Range(-1, 2);
        Vector2 randomDir = new Vector2(randomX, randomY);

        randomDir.Normalize();

        float randNum = Random.Range(0, maxThrowSpeed);
        float throwSpeed = randNum * 50;
        bubble.ImpulseItem(randomDir, throwSpeed);
        bubble.transform.up = randomDir;

        bubble.GetComponentInChildren<UnityEngine.Rendering.Universal.Light2D>().color = currentDrop.EffectsColor;
    }

    #endregion

    protected virtual void EnemyPause()
    {
        rb2d.velocity = Vector2.zero;
        rb2d.angularVelocity = 0.0f;
    }

    protected virtual void EnemyResume()
    {

    }

    protected void OnDrawGizmosSelected()
    {
        if (showGizmos == false)
            return;

        foreach (Transform spot in moveSpots)
        {
            if (spot)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(spot.position, 0.2f);
            }
        }
    }
}
