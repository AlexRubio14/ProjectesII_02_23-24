using UnityEngine;

public class Enemy01 : Enemy
{
    [Space, Header("--- ENEMY 01"), SerializeField]
    public float eatingDuration;
    [SerializeField]
    public float eatingForce; 
    [SerializeField]
    public int eatingHeal;
    [SerializeField]
    private ParticleSystem eatingParticles;

    [SerializeField]
    private AudioClip impactPlayerClip;

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
        if(iaData.m_currentTarget == null)
        {
            if(moveSpots.Length > 0)
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
        if(iaData.m_currentTarget == null)
        {
            ChangeState(EnemyStates.PATROLLING);
            return; 
        }
        MoveToTarget(); 
    }

    protected void StopEating()
    {
        ChangeState(EnemyStates.CHASING);
    }
    private void StartEating(Vector2 collisionPoint)
    {
        currentHealth = Mathf.Clamp(currentHealth + eatingHeal, 0, maxHealth); 
        
        eatingParticles.Play();
        StartKnockback(collisionPoint, eatingForce);
        ChangeState(EnemyStates.EXTRA);
        Invoke("StopEating", eatingDuration); 
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
                rb2d.velocity = Vector2.zero;
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
            AudioManager.instance.Play2dOneShotSound(impactPlayerClip, "Enemy");
            StartEating(collision.contacts[0].point); 
        }
        else if (collision.collider.CompareTag(BULLET_TAG) && currentState != EnemyStates.EXTRA)
        {
            float bulletDamage = collision.collider.GetComponent<Laser>().GetBulletDamage();
            GetHit(bulletDamage);
            ChangeState(EnemyStates.KNOCKBACK);
            StartKnockback(collision.transform.position, knockbackForce);
        }
    }
}
