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
    public WeaponSlotAssignment[] _weaponSlots;
    public Weapon[] _availableWeapons;

    private Weapon _currentWeapon;

    public void OnEnable()
    {
        _currentWeapon = _availableWeapons[0];
    }

    public void ShootPrimary()
    {
        if (_currentWeapon.CurrentAmmo > 0)
        {
            var simultBullet = _currentWeapon.WeaponSlotSpawns.Length;
            _currentWeapon.CurrentAmmo -= simultBullet;

            for (int i = 0; i < simultBullet; i++)
            {
                var bullet = ObjectPoolManager.Spawn(
                    _currentWeapon.BulletPF, 
                    GetLocationOfWeaponSlot(_currentWeapon.WeaponSlotSpawns[i]), 
                    transform.rotation
                );

                // bullet.SetSpeed;
                // bullet.SetDamage;
                // bullet.SetLifetime;
                bullet.GetComponentInChildren<Rigidbody>().AddForce(10000f * transform.forward, ForceMode.Force);
            }

            if (_currentWeapon.CurrentAmmo <= 0)
            {
                Reload();
            }
        }
    }

    public void ShootSecondary()
    {
        Debug.Log("Shoot Secondary");
    }

    public void Reload()
    {
        _currentWeapon.CurrentAmmo = _currentWeapon.MagSize;
    }

    public void ChangeWeapon()
    {

    }

    private Vector3 GetLocationOfWeaponSlot(WeaponSlot weaponSlotType)
    {
        foreach (var assignedSlot in _weaponSlots)
        {
            if (assignedSlot.WeaponSlotType == weaponSlotType)
            {
                return assignedSlot.WeaponSlotTransform.position;
            }
        }

        return transform.position;
    }
}
