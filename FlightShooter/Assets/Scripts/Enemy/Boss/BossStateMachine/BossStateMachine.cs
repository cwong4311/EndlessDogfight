using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStateMachine
{
    public BossState CurrentBossState { get; set; }

    public void Initialise(BossState startingState)
    {
        CurrentBossState = startingState;
        CurrentBossState.OnStateEnter();
    }

    public void ChangeState(BossState nextState)
    {
        CurrentBossState.OnStateExit();
        CurrentBossState = nextState;
        CurrentBossState.OnStateEnter();
    }
}
