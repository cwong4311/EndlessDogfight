using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public struct BossWeapon
{
    public EnemyWeapon Weapon;
    public bool AutomaticFire;
}

public class BossController : MonoBehaviour
{
    public BossWeapon[] WeaponManager;

    public Transform PlayerTarget;
    public float FiringRange;

    public float MovementSpeed;

    [SerializeField]
    private EnemyHealth _bossHealth;

    private float _sqrFiringRange;

    #region StateMachine
    public BossStateMachine BossStateMachine;

    public IdleBossState IdleState;
    public MovingBossState MovingState;
    public FiringBossState FiringState;
    #endregion

    public void OnEnable()
    {
        _sqrFiringRange = FiringRange * FiringRange;

        BossStateMachine = new BossStateMachine();
        IdleState = new IdleBossState(this, BossStateMachine);
        MovingState = new MovingBossState(this, BossStateMachine, MovementSpeed);
        FiringState = new FiringBossState(this, BossStateMachine, FiringRange);

        BossStateMachine.Initialise(IdleState);

        BGMController.OnBossSpawn.Invoke();
    }

    private void OnDisable()
    {
        try
        {
            BGMController.OnBossDefeat.Invoke();
        }
        catch { } // Do nothing, this can fail
    }

    public void SetTarget(Transform player)
    {
        PlayerTarget = player;
    }

    public void Buff(float modifier)
    {
        foreach (var enemyWeapon in WeaponManager)
        {
            if (enemyWeapon.Weapon != null)
            {
                enemyWeapon.Weapon.DamageMod = 1 + modifier;
            }
        }

        if (_bossHealth != null)
        {
            _bossHealth.MaxHealth *= (1 + modifier);
            _bossHealth.CurrentHealth *= (1 + modifier);
        }
    }

    private void Update()
    {
        BossStateMachine.CurrentBossState.Update();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (var enemyWeapon in WeaponManager)
        {
            if (enemyWeapon.AutomaticFire && CanFire())
            {
                enemyWeapon.Weapon.ShootWeapon();
            }
        }

        BossStateMachine.CurrentBossState.FixedUpdate();
    }

    private bool CanFire()
    {
        var dirToPlayer = transform.position - PlayerTarget.position;
        if (dirToPlayer.sqrMagnitude < _sqrFiringRange)
        {
            return true;
        }

        return false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, FiringRange);
    }
}
