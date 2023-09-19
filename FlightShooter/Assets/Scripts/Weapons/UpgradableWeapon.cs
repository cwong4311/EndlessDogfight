using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class UpgradableWeapon : ScriptableObject
{
    private Weapon _currentWeapon;
    private int _currentLevel; 

    [SerializeField]
    public Weapon[] AllWeaponLevels;

    public void Ready()
    {
        if (AllWeaponLevels.Length < 1)
        {
            throw new MissingReferenceException(this.name + " is missing a single weapon level");
        }

        ResetLevel();
    }

    public void ResetLevel()
    {
        _currentLevel = 0;
        _currentWeapon = AllWeaponLevels[0];

        _currentWeapon.CurrentAmmo = _currentWeapon.MagSize;
    }

    public void LevelUp()
    {
        if (_currentLevel < AllWeaponLevels.Length - 1)
        {
            var keepCurrentAmmo = _currentWeapon.CurrentAmmo;

            _currentLevel++;
            _currentWeapon = AllWeaponLevels[_currentLevel];

            _currentWeapon.CurrentAmmo = keepCurrentAmmo;
        }
    }

    public Weapon GetWeapon()
    {
        return _currentWeapon;
    }



    #region Accessors
    public string WeaponName
    { 
        get { return _currentWeapon.WeaponName; } 
        set { _currentWeapon.WeaponName = value; } 
    }

    public float CurrentAmmo
    {
        get { return _currentWeapon.CurrentAmmo; }
        set { _currentWeapon.CurrentAmmo = value; }
    }

    public float MagSize
    {
        get { return _currentWeapon.MagSize; }
        set { _currentWeapon.MagSize = value; }
    }

    public float Damage
    {
        get { return _currentWeapon.Damage; }
        set { _currentWeapon.Damage = value; }
    }

    public float RateOfFire
    {
        get { return _currentWeapon.RateOfFire; }
        set { _currentWeapon.RateOfFire = value; }
    }

    public float BulletsPerShot
    {
        get { return _currentWeapon.BulletsPerShot; }
        set { _currentWeapon.BulletsPerShot = value; }
    }

    public GameObject BulletPF
    {
        get { return _currentWeapon.BulletPF; }
        set { _currentWeapon.BulletPF = value; }
    }

    public float ReloadTime
    {
        get { return _currentWeapon.ReloadTime; }
        set { _currentWeapon.ReloadTime = value; }
    }

    public float BulletSpeed
    {
        get { return _currentWeapon.BulletSpeed; }
        set { _currentWeapon.BulletSpeed = value; }
    }

    public float BulletTravelDistance
    {
        get { return _currentWeapon.BulletTravelDistance; }
        set { _currentWeapon.BulletTravelDistance = value; }
    }

    public WeaponSlot[] WeaponSlotSpawns
    {
        get { return _currentWeapon.WeaponSlotSpawns; }
        set { _currentWeapon.WeaponSlotSpawns = value; }
    }
    #endregion
}
