using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossState
{
    protected BossController BossController;

    protected BossStateMachine BossStateMachine;

    public BossState(BossController bossController, BossStateMachine bossStateMachine)
    {
        BossController = bossController;
        BossStateMachine = bossStateMachine;
    }

    public abstract void OnStateEnter();

    public abstract void OnStateExit();

    public abstract void Update();

    public abstract void FixedUpdate();
}
