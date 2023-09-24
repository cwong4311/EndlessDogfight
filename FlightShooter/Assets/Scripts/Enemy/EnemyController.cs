using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyWeapon WeaponManager;

    public float FiringRange;
    public float FiringAngle;

    public Transform PlayerTarget;
    private float _sqrFiringRange;

    public void OnEnable()
    {
        _sqrFiringRange = FiringRange * FiringRange;
    }

    public void SetTarget(Transform player)
    {
        PlayerTarget = player;
    }

    public void Buff(float modifier)
    {
        var enemyWeapon = WeaponManager as EnemyWeapon;
        if (enemyWeapon != null)
        {
            enemyWeapon.DamageMod = 1 + modifier;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (CanFire())
        {
            WeaponManager.ShootWeapon();
        }
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
