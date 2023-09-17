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
    Back,
    FarWing_L,
    FarWing_R,
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
    public Weapon[] _availablePrimaryWeapons;
    public Weapon[] _availableSecondaryWeapons;

    private Weapon _currentWeapon;
    private Weapon _secondaryWeapon;

    private bool _isShootCooldown;
    private float _isShootStartTime;

    private bool _isReloading;
    private float _reloadStartTime;

    private bool _isSecondaryCD;
    private float _secondaryStartTime;

    private bool _isSecondaryReload;
    private float _secondaryReloadStart;

    public void OnEnable()
    {
        _currentWeapon = _availablePrimaryWeapons[0];
        _currentWeapon.CurrentAmmo = _currentWeapon.MagSize;

        _secondaryWeapon = _availableSecondaryWeapons[0];
        _secondaryWeapon.CurrentAmmo = _secondaryWeapon.MagSize;

        _isShootCooldown = false;
        _isReloading = false;
        _isSecondaryCD = false;
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

        if (_isSecondaryCD)
        {
            if (Time.time - _secondaryStartTime >= (1 / _secondaryWeapon.RateOfFire))
            {
                _isSecondaryCD = false;
            }
        }

        if (_isSecondaryReload)
        {
            if (Time.time - _secondaryReloadStart >= _secondaryWeapon.ReloadTime)
            {
                _secondaryWeapon.CurrentAmmo = _secondaryWeapon.MagSize;
                _isSecondaryReload = false;
            }
        }
    }

    public void ShootPrimary()
    {
        ShootWeapon(_currentWeapon, _isShootCooldown, OverheatPrimary, ReloadPrimary);
    }

    public void ShootSecondary()
    {
        ShootWeapon(_secondaryWeapon, _isSecondaryCD, OverheatSecondary, ReloadSecondary);
    }

    public void ShootWeapon(Weapon targetWeapon, bool weaponOnCooldown, Action OnOverheat, Action OnReload)
    {
        if (targetWeapon.CurrentAmmo > 0 && !weaponOnCooldown)
        {
            var bulletInBurst = targetWeapon.BulletsPerShot > 0 ? targetWeapon.BulletsPerShot : 1;
            var simultBullet = targetWeapon.WeaponSlotSpawns.Length;
            targetWeapon.CurrentAmmo -= (simultBullet * bulletInBurst);

            for (int i = 0; i < simultBullet; i++)
            {
                for (int n = 0; n < bulletInBurst; n++)
                {
                    var bulletNozzle = GetTransformOfWeaponSlot(targetWeapon.WeaponSlotSpawns[i]);

                    var bullet = ObjectPoolManager.Spawn(
                        targetWeapon.BulletPF,
                        bulletNozzle.position,
                        bulletNozzle.rotation
                    ).GetComponent<IProjectile>();

                    bullet.Force = targetWeapon.BulletSpeed;
                    bullet.Damage = targetWeapon.Damage;
                    bullet.Direction = bulletNozzle.forward;
                    bullet.LifeTime = targetWeapon.BulletTravelDistance / targetWeapon.BulletSpeed;
                    bullet.TargetColliders = _enemyLayers;
                    bullet.SetLayer(LayerMask.NameToLayer("Player"));

                    bullet.Fire();
                }
            }

            OnOverheat();
        }

        if (targetWeapon.CurrentAmmo <= 0)
        {
            OnReload();
        }
    }

    private void OverheatPrimary()
    {
        if (_isShootCooldown == false)
        {
            _isShootCooldown = true;
            _isShootStartTime = Time.time;
        }
    }

    public void ReloadPrimary()
    {
        if (_isReloading == false)
        {
            _isReloading = true;
            _reloadStartTime = Time.time;
        }
    }

    private void OverheatSecondary()
    {
        if (_isSecondaryCD == false)
        {
            _isSecondaryCD = true;
            _secondaryStartTime = Time.time;
        }
    }

    public void ReloadSecondary()
    {
        if (_isSecondaryReload == false)
        {
            _isSecondaryReload = true;
            _secondaryReloadStart = Time.time;
        }
    }

    public void ChangeWeapon()
    {
        // TO DO
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
