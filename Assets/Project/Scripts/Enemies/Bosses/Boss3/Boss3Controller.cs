using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3Controller : BossController
{

    private void Update()
    {
        
    }

    protected override void SetupPhaseAttacks()
    {
        List<Action> startActions = new List<Action>();
        List<Action> updateActions = new List<Action>();

        

        onStartPhaseAttacks.Add(Phase.PHASE_1, startActions.ToArray());
        onUpdatePhaseAttacks.Add(Phase.PHASE_1, updateActions.ToArray());


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

    public override void GetDamage(float _damage)
    {
        throw new System.NotImplementedException();
    }

    protected override void StartDie()
    {
        throw new System.NotImplementedException();
    }

    protected override void UpdateDie()
    {
        throw new System.NotImplementedException();
    }

}
