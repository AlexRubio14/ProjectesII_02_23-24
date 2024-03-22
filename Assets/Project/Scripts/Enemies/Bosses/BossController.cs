using AYellowpaper.SerializedCollections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BossController : MonoBehaviour
{
    [Header("Health"), SerializeField]
    protected float maxHealth;
    protected float currentHealth;
    [Space, SerializeField]
    protected Slider healthBar;

    public enum Phase { PHASE_1, PHASE_2, DEAD};
    [Space, Header("Phases"), SerializeField]
    protected Phase currentPhase;
    [SerializedDictionary("Phase", "LastPercentage")]
    public SerializedDictionary<Phase, float> phaseChangerPercentage;
    protected Dictionary<Phase, Action[]> onStartPhaseAttacks;
    protected Dictionary<Phase, Action[]> onUpdatePhaseAttacks;

    protected int currentAttackID;
    protected int lastAttack;
    protected Action onDieStart;
    protected Action onDieUpdate;

    protected Rigidbody2D rb2d;


    protected void Start()
    {
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        UpdateHealthBar();

        //currentPhase = Phase.PHASE_1;
        currentAttackID = -1;
        lastAttack = -1;
        onStartPhaseAttacks = new Dictionary<Phase, Action[]>();
        onUpdatePhaseAttacks = new Dictionary<Phase, Action[]>();

        SetupPhaseAttacks();

        GenerateRandomAttack();
    }

    protected abstract void SetupPhaseAttacks();

    protected void CheckPhase()
    {
        float percentage = maxHealth * (phaseChangerPercentage[currentPhase] / 100);

        if (currentHealth <= percentage && currentPhase != Phase.DEAD)
        {
            currentPhase++;
            ChangePhase(currentPhase);
        }
    }
    protected void ChangePhase(Phase _nextPhase)
    {
        currentPhase = _nextPhase;

        currentAttackID = -1;
        lastAttack = -1;
    }

    protected void GenerateRandomAttack()
    {
        CheckPhase();

        int nextAttackId = UnityEngine.Random.Range(0, onStartPhaseAttacks[currentPhase].Length);

        if (nextAttackId == currentAttackID || nextAttackId == lastAttack)
        {
            GenerateRandomAttack();
            return;
        }

        ChangeAttack(nextAttackId);
    }
    protected void ChangeAttack(int _attackID)
    {
        lastAttack = currentAttackID;
        currentAttackID = _attackID;

        if(onStartPhaseAttacks[currentPhase][currentAttackID] != null)
            onStartPhaseAttacks[currentPhase][currentAttackID]();
    }
    public abstract void GetDamage(float _damage);
    protected void UpdateHealthBar()
    {
        healthBar.value = currentHealth;
    }

}
