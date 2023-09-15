using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public enum WeaponSlot
{
    Main,
    Side_L,
    Side_R,
    Wing_L,
    Wing_R,
    Back
}

[Serializable]
public class WeaponSlotAssignment
{
    public WeaponSlot WeaponSlotType;
    public Transform WeaponSlotTransform;
}

public class WeaponsMananger : MonoBehaviour
{
    public LayerMask _enemyLayers;

    public WeaponSlotAssignment[] _weaponSlots;
    public Weapon[] _availableWeapons;

    private Weapon _currentWeapon;

    private bool _isShootCooldown;
    private float _isShootStartTime;

    private bool _isReloading;
    private float _reloadStartTime;

    public void OnEnable()
    {
        _currentWeapon = _availableWeapons[0];
        _currentWeapon.CurrentAmmo = _currentWeapon.MagSize;

        _isShootCooldown = false;
        _isReloading = false;
    }

    public void Update()
    {
        if (_isShootCooldown)
        {
            if (Time.time - _isShootStartTime >= (1 / _currentWeapon.RateOfFire))
            {
                _isShootCooldown = false;
            }
        }

        if (_isReloading)
        {
            if (Time.time - _reloadStartTime >= _currentWeapon.ReloadTime)
            {
                _currentWeapon.CurrentAmmo = _currentWeapon.MagSize;
                _isReloading = false;
            }
        }
    }

    public void ShootPrimary()
    {
        if (_currentWeapon.CurrentAmmo > 0 && !_isShootCooldown)
        {
            var simultBullet = _currentWeapon.WeaponSlotSpawns.Length;
            _currentWeapon.CurrentAmmo -= simultBullet;

            for (int i = 0; i < simultBullet; i++)
            {
                var bulletNozzle = GetTransformOfWeaponSlot(_currentWeapon.WeaponSlotSpawns[i]);

                var bullet = ObjectPoolManager.Spawn(
                    _currentWeapon.BulletPF,
                    bulletNozzle.position,
                    transform.rotation
                ).GetComponent<IProjectile>();

                bullet.Force = _currentWeapon.BulletSpeed;
                bullet.Damage = _currentWeapon.Damage;
                bullet.Direction = bulletNozzle.forward;
                bullet.LifeTime = 3f;
                bullet.TargetColliders = _enemyLayers;
                bullet.SetLayer(LayerMask.NameToLayer("Player"));

                bullet.Fire();
            }

            Overheat();
        }

        if (_currentWeapon.CurrentAmmo <= 0)
        {
            Reload();
        }
    }

    public void ShootSecondary()
    {
        Debug.Log("Shoot Secondary");
    }

    private void Overheat()
    {
        if (_isShootCooldown == false)
        {
            _isShootCooldown = true;
            _isShootStartTime = Time.time;
        }
    }

    public void Reload()
    {
        if (_isReloading == false)
        {
            _isReloading = true;
            _reloadStartTime = Time.time;
        }
    }

    public void ChangeWeapon()
    {

    }

    private Transform GetTransformOfWeaponSlot(WeaponSlot weaponSlotType)
    {
        foreach (var assignedSlot in _weaponSlots)
        {
            if (assignedSlot.WeaponSlotType == weaponSlotType)
            {
                return assignedSlot.WeaponSlotTransform;
            }
        }

        return transform;
    }
}
