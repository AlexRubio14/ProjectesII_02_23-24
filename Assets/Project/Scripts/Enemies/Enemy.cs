using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class Enemy : EnemyIA, IHealth
{
    [SerializeField]
    protected int maxHealth;
    protected int currentHealth;

    protected bool isDead;
    [SerializeField]
    protected float damage;

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
            isDead = true;
            Destroy(gameObject);
        }
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
