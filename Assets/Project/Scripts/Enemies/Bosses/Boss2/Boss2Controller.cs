using System;
using System.Collections.Generic;
using UnityEngine;

public class Boss2Controller : BossController
{

    private Animator animator;


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    
    void Update()
    {
        if (onUpdatePhaseAttacks[currentPhase][currentAttackID] != null)
            onUpdatePhaseAttacks[currentPhase][currentAttackID]();

        CheckIfDead();
    }

    protected override void SetupPhaseAttacks()
    {
        //Fase 1
        List<Action> startActions = new List<Action>();
        List<Action> updateActions = new List<Action>();

        //dashStart += StartDashWallToWall;
        //startActions.Add(dashStart);
        //dashUpdate += UpdateDashWallToWall;
        //updateActions.Add(dashUpdate);

        

        onStartPhaseAttacks.Add(Phase.PHASE_1, startActions.ToArray());
        onUpdatePhaseAttacks.Add(Phase.PHASE_1, updateActions.ToArray());

        //Fase 2
        startActions = new List<Action>();
        updateActions = new List<Action>();

        onStartPhaseAttacks.Add(Phase.PHASE_2, startActions.ToArray());
        onUpdatePhaseAttacks.Add(Phase.PHASE_2, updateActions.ToArray());

        //Cuando este muerto
        startActions = new List<Action>();
        updateActions = new List<Action>();


        onDieStart += StartDie;
        startActions.Add(onDieStart);
        onDieUpdate += UpdateDie;
        updateActions.Add(onDieUpdate);

        onStartPhaseAttacks.Add(Phase.DEAD, startActions.ToArray());
        onUpdatePhaseAttacks.Add(Phase.DEAD, updateActions.ToArray());
    }

    #region Attack 1

    #endregion

    #region Attack 2

    #endregion

    #region Attack 3

    #endregion



    public override void GetDamage(float _damage)
    {
        currentHealth -= _damage;

        UpdateHealthBar();
    }

    #region Die
    protected override void StartDie()
    {
        
    }

    protected override void UpdateDie()
    {
        
    }
    #endregion

}
