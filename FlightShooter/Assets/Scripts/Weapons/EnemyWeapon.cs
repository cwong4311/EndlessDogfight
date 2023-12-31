using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;


public class EnemyWeapon : MonoBehaviour, IWeaponManager
{
    public LayerMask TargetLayers;

    public Transform[] ShootSlots;
    public float Damage;
    public float RateOfFire;
    public GameObject BulletPF;
    public AudioClip BulletSound;

    public float BulletSpeed;
    public float BulletTravelDistance;

    private bool _isShootCooldown;
    private float _isShootStartTime;

    public AudioSource AudioSourceBullet;

    public float DamageMod = 1f;
    public bool IsInCooldown => _isShootCooldown;


    public void Update()
    {
        if (_isShootCooldown)
        {
            if (Time.time - _isShootStartTime >= (1 / RateOfFire))
            {
                _isShootCooldown = false;
            }
        }
    }
    public virtual void ShootWeapon()
    {
        if (!_isShootCooldown)
        {
            for (int i = 0; i < ShootSlots.Length; i++)
            {
                var bulletNozzle = ShootSlots[i];

                var bullet = ObjectPoolManager.Spawn(
                    BulletPF,
                    bulletNozzle.position,
                    bulletNozzle.rotation
                ).GetComponent<IProjectile>();

                bullet.Force = BulletSpeed;
                bullet.Damage = Damage * DamageMod;
                bullet.Direction = bulletNozzle.forward;
                bullet.LifeTime = BulletTravelDistance / BulletSpeed;
                bullet.TargetColliders = TargetLayers;
                bullet.SetLayer(LayerMask.NameToLayer("EnemyBullet"));

                bullet.Fire();
                PlayBulletSound();
            }

            OverheatPrimary();
        }
    }

    private void PlayBulletSound()
    {
        if (AudioSourceBullet != null && BulletSound != null)
        {
            AudioSourceBullet.PlayOneShot(BulletSound);
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
}
