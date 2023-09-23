using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyWeapon WeaponManager;

    public float FiringRange;
    public float FiringAngle;

    public Transform _playerTarget;
    private float _sqrFiringRange;

    public void OnEnable()
    {
        _sqrFiringRange = FiringRange * FiringRange;
    }

    public void SetTarget(Transform player)
    {
        _playerTarget = player;
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
        var dirToPlayer = transform.position - _playerTarget.position;
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
