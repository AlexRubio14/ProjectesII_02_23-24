using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class Enemy : EnemyIA, IHealth
{
    public enum EnemyStates { PATROLLING, CHASING, KNOCKBACK, EATING }
    public EnemyStates currentState = EnemyStates.PATROLLING;

    [Space, Header("Base Enemy"), SerializeField]
    protected float maxHealth;
    protected float currentHealth;

    protected bool isDead;
    [SerializeField]
    protected float damage;
    [Header("Drop"), SerializeField]
    protected ItemObject c_currentDrop;
    [SerializeField]
    protected GameObject c_pickableItemPrefab;
    [SerializeField]
    protected float maxThrowSpeed;

    protected string BULLET_TAG = "Bullet";

    #region Behaviours Functions
    protected abstract void Behaviour();
    protected abstract void PatrollingBehaviour(); 
    protected abstract void ChaseBehaviour();
    #endregion

    #region States Functions
    protected abstract void ChangeState(EnemyStates nextState);
    protected abstract void CheckState();

    public EnemyStates GetState()
    {
        return currentState;
    }
    #endregion

    #region Damage Functions
    public void GetHit(float _damageAmount)
    {
        currentHealth -= _damageAmount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    virtual protected void Die()
    {
        if (c_currentDrop)
        {
            DropItem();
        }

        isDead = true;
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
    public float GetDamage()
    {
        return damage;
    }
    #endregion


    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(BULLET_TAG))
        {
            float bulletDamage = collision.GetComponent<Laser>().GetBulletDamage();
            GetHit(bulletDamage); ;
        }
    }
}
