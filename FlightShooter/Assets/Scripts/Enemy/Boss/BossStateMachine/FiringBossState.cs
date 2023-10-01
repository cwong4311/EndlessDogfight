using UnityEngine;
using System.Linq;

public class FiringBossState : BossState
{
    private float _firingRange;
    private float _firingDelay;
    private BossWeapon[] _managedWeapons;

    private float _minFiringDelay = 3f;
    private float _maxFiringDelay = 7f;

    public FiringBossState(BossController bossController, BossStateMachine bossStateMachine, float firingRange) : base(bossController, bossStateMachine)
    {
        _firingRange = firingRange;
        _managedWeapons = bossController.WeaponManager.Where(e => e.AutomaticFire == false).ToArray();
    }

    public override void OnStateEnter()
    {
        _firingDelay = Random.Range(_minFiringDelay, _maxFiringDelay);
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
        _firingDelay -= Time.fixedDeltaTime;
        if (_firingDelay <= 0f)
        {
            BossController.BossStateMachine.ChangeState(BossController.IdleState);
        }

        foreach (var bossWeapon in _managedWeapons)
        {
            if (bossWeapon.Weapon.IsInCooldown == false && InFiringRange())
            {
                foreach (var bulletNozzle in bossWeapon.Weapon.ShootSlots)
                {
                    bulletNozzle.LookAt(BossController.PlayerTarget);
                }

                bossWeapon.Weapon.ShootWeapon();
            }
        }
    }

    private bool InFiringRange()
    {
        if ((BossController.transform.position - BossController.PlayerTarget.position).sqrMagnitude <= _firingRange * _firingRange)
        {
            return true;
        }

        return false;
    }

    private void LookAtTarget()
    {
        var lookVector = BossController.PlayerTarget.position - BossController.transform.position;
        if (lookVector != Vector3.zero)
        {
            var lookRotation = Quaternion.LookRotation(lookVector);
            BossController.transform.rotation = Quaternion.Slerp(BossController.transform.rotation, lookRotation, 1f * Time.fixedDeltaTime);
        }
    }
}
