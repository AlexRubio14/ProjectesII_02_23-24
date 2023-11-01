using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : EnemyIA
{
    protected int currentHealth;
    [SerializeField]
    protected int maxHealth;

    protected bool isDead;

    protected abstract void CheckState();
    protected abstract void Behaviour();
    protected abstract void PatrollingBehaviour(); 
    protected abstract void ChaseBehaviour();
}
