using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleBossState : BossState
{
    private float _idleTime;

    private float _minIdleDelay = 1f;
    private float _maxIdleDelay = 3f;

    public IdleBossState(BossController bossController, BossStateMachine bossStateMachine) : base(bossController, bossStateMachine) { }

    public override void OnStateEnter()
    {
        _idleTime = Random.Range(_minIdleDelay, _maxIdleDelay);
    }

    public override void OnStateExit()
    {
        // Do nothing
    }

    public override void Update()
    {
        // Do nothing
    }

    public override void FixedUpdate()
    {
        LookAtTarget();

        _idleTime -= Time.fixedDeltaTime;
        if (_idleTime <= 0f)
        {
            var rand = Random.Range(0, 2);
            if (rand > 0)
            {
                BossController.BossStateMachine.ChangeState(BossController.FiringState);
            }
            else
            {
                BossController.BossStateMachine.ChangeState(BossController.MovingState);
            }
        }
    }

    private void LookAtTarget()
    {
        var lookVector = BossController.PlayerTarget.position - BossController.transform.position;
        if (lookVector != Vector3.zero)
        {
            var lookRotation = Quaternion.LookRotation(lookVector);
            BossController.transform.rotation = Quaternion.Slerp(BossController.transform.rotation, lookRotation, 2f * Time.fixedDeltaTime);
        }
    }
}
