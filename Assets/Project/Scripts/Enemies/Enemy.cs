using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class Enemy : EnemyIA, IHealth
{
    [Space, Header("Base Enemy"), SerializeField]
    protected int maxHealth;
    protected int currentHealth;

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

    protected abstract void CheckState();
    protected abstract void Behaviour();
    protected abstract void PatrollingBehaviour(); 
    protected abstract void ChaseBehaviour();
    public void GetHit(int amount)
    {
        currentHealth -= amount;

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

    protected void BulletCollision(Collider2D collision, int bulletDamage)
    {
        if (collision.CompareTag(BULLET_TAG))
        {
            GetHit(bulletDamage);
        }
    }
    public float GetDamage()
    {
        return damage;
    }
}
