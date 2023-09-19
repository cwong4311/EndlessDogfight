using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectile
{
    public float Damage { get; set; }

    public float Force { get; set; }

    public float LifeTime { get; set; }

    public Vector3 Direction { get; set; }

    public LayerMask TargetColliders { get; set; }


    public void Fire();

    public void DoDamage(IHealth enemyHealth);

    public void SetLayer(int layer);

    public void OnCollisionEnter(Collision collision);

    public void Destroy();
}
