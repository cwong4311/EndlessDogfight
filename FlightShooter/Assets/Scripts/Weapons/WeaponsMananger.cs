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

public class WeaponsMananger : MonoBehaviour, IWeaponManager
{
    public LayerMask _enemyLayers;

    public WeaponSlotAssignment[] _weaponSlots;
    public UpgradableWeapon[] _availablePrimaryWeapons;
    public Weapon[] _availableSecondaryWeapons;

    private UpgradableWeapon _currentWeapon;
    private Weapon _secondaryWeapon;

    private bool _isShootCooldown;
    private float _isShootStartTime;

    private bool _isReloading;
    private float _reloadStartTime;

    private bool _isSecondaryCD;
    private float _secondaryStartTime;

    private bool _isSecondaryReload;
    private float _secondaryReloadStart;

    public RectTransform PrimaryAmmoBar;
    public RectTransform SecondaryAmmoBar;
    private float _originalPrimaryAmmoSize;
    private float _originalSecondaryAmmoSize;

    public void OnEnable()
    {
        foreach (var _primaryWeapon in _availablePrimaryWeapons)
        {
            _primaryWeapon.Ready();
        }

        _currentWeapon = _availablePrimaryWeapons[0];
        _currentWeapon.CurrentAmmo = _currentWeapon.MagSize;

        _secondaryWeapon = _availableSecondaryWeapons[0];
        _secondaryWeapon.CurrentAmmo = _secondaryWeapon.MagSize;

        _isShootCooldown = false;
        _isReloading = false;
        _isSecondaryCD = false;

        _originalPrimaryAmmoSize = PrimaryAmmoBar.sizeDelta.x;
        _originalSecondaryAmmoSize = SecondaryAmmoBar.sizeDelta.x;
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
            UpdatePrimaryAmmoBar(_isReloading);

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
            UpdateSecondaryAmmoBar(_isSecondaryReload);

            if (Time.time - _secondaryReloadStart >= _secondaryWeapon.ReloadTime)
            {
                _secondaryWeapon.CurrentAmmo = _secondaryWeapon.MagSize;
                _isSecondaryReload = false;
            }
        }
    }

    public void ShootPrimary()
    {
        ShootWeapon(_currentWeapon.GetWeapon(), _isShootCooldown, OverheatPrimary, ReloadPrimary);
        UpdatePrimaryAmmoBar(false);
    }

    public void ShootSecondary()
    {
        ShootWeapon(_secondaryWeapon, _isSecondaryCD, OverheatSecondary, ReloadSecondary);
        UpdateSecondaryAmmoBar(false);
    }

    public void ShootWeapon()
    {
        // Not used for Player's weapon manager
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
                    bullet.SetLayer(LayerMask.NameToLayer("PlayerBullet"));

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

    public void ChangeWeapon(string weaponName)
    {
        if (weaponName.Equals(_currentWeapon.WeaponName))
        {
            _currentWeapon.LevelUp();
        }
        else
        {
            _currentWeapon.ResetLevel();
            _currentWeapon = LoadWeapon(weaponName);
            
            _currentWeapon.CurrentAmmo = 0;
            ReloadPrimary();
        }
    }

    public void ChangeSecondary(string secondaryName)
    {
        if (secondaryName.Equals(_secondaryWeapon.WeaponName) == false)
        {
            _secondaryWeapon = LoadSecondary(secondaryName);

            _secondaryWeapon.CurrentAmmo = 0;
            ReloadSecondary();
        }
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

    private UpgradableWeapon LoadWeapon(string weaponName)
    {
        foreach (var upgradableWeapon in _availablePrimaryWeapons)
        {
            if (upgradableWeapon.WeaponName.Equals(weaponName))
            {
                return upgradableWeapon;
            }
        }

        return _availablePrimaryWeapons[0];
    }

    private Weapon LoadSecondary(string weaponName)
    {
        foreach (var weapon in _availableSecondaryWeapons)
        {
            if (weapon.WeaponName.Equals(weaponName))
            {
                return weapon;
            }
        }

        return _availableSecondaryWeapons[0];
    }

    private void UpdatePrimaryAmmoBar(bool isReloading)
    {
        if (isReloading)
        {
            PrimaryAmmoBar.sizeDelta = new Vector2(
                _originalPrimaryAmmoSize * ((Time.time - _reloadStartTime) / _currentWeapon.ReloadTime),
                PrimaryAmmoBar.sizeDelta.y
            );
        }
        else
        {
            PrimaryAmmoBar.sizeDelta = new Vector2(
                _originalPrimaryAmmoSize * (_currentWeapon.CurrentAmmo / _currentWeapon.MagSize),
                PrimaryAmmoBar.sizeDelta.y
            );
        }
    }

    private void UpdateSecondaryAmmoBar(bool isReloading)
    {
        if (isReloading)
        {
            SecondaryAmmoBar.sizeDelta = new Vector2(
                _originalSecondaryAmmoSize * ((Time.time - _secondaryReloadStart) / _secondaryWeapon.ReloadTime),
                SecondaryAmmoBar.sizeDelta.y
            );
        }
        else
        {
            SecondaryAmmoBar.sizeDelta = new Vector2(
                _originalSecondaryAmmoSize * (_secondaryWeapon.CurrentAmmo / _secondaryWeapon.MagSize),
                SecondaryAmmoBar.sizeDelta.y
            );
        }
    }
}
