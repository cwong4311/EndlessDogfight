using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBossState : BossState
{
    private float _movementSpeed;
    private Vector3 _movementLocation;

    private bool _lookAtPlayer;

    public MovingBossState(BossController bossController, BossStateMachine bossStateMachine, float MovementSpeed) : base(bossController, bossStateMachine)
    {
        _movementSpeed = MovementSpeed;
    }

    public override void OnStateEnter()
    {
        var playerLocation = BossController.PlayerTarget.position;
        var avoidRadius = 150f;
        var travelRadius = 600f;

        Vector3 dir = Random.insideUnitSphere;
        float length = avoidRadius + travelRadius * Random.value;

        _movementLocation = playerLocation + (dir.normalized * length);
        if (_movementLocation.y < 0f)
        {
            _movementLocation = new Vector3(_movementLocation.x, Random.Range(50f, 100f), _movementLocation.z);
        }

        _lookAtPlayer = (Random.Range(0, 3) == 0) ? true : false;
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

        if ((BossController.transform.position - _movementLocation).sqrMagnitude > 0.1f)
        {
            BossController.transform.position = Vector3.MoveTowards(
                BossController.transform.position,
                _movementLocation,
                _movementSpeed * Time.fixedDeltaTime);
        }
        else
        {
            BossController.BossStateMachine.ChangeState(BossController.IdleState);
        }
    }

    private void LookAtTarget()
    {
        var lookAtTarget = _lookAtPlayer ? BossController.PlayerTarget.position : _movementLocation;
        var lookVector = lookAtTarget - BossController.transform.position;
        if (lookVector != Vector3.zero)
        {
            var lookRotation = Quaternion.LookRotation(lookVector);
            BossController.transform.rotation = Quaternion.Slerp(BossController.transform.rotation, lookRotation, 3f * Time.fixedDeltaTime);
        }
    }
}
