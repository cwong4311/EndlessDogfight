using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSecondary : MonoBehaviour, IProjectile
{
    public float Damage { get; set; }

    public float Force { get; set; }

    public float LifeTime { get; set; }

    public Vector3 Direction { get; set; }

    public LayerMask TargetColliders { get; set; }

    public float ShieldRange;
    public float HealAmount;
    public bool TemporaryInvuln;

    private float _aliveSince;
    private IHealth _userHealth;

    public void OnEnable()
    {
        _aliveSince = Time.time;
    }

    public void Update()
    {
        if (Time.time - _aliveSince >= LifeTime)
        {
            Destroy();
        }
    }

    public void FixedUpdate()
    {
        var listOfColliders = Physics.OverlapSphere(transform.position, ShieldRange);

        foreach (var potentialEnemies in listOfColliders)
        {
            if (((1 << potentialEnemies.gameObject.layer) & TargetColliders.value) != 0
                && potentialEnemies.TryGetComponent<IProjectile>(out var _enemyProjectile))
            {
                _enemyProjectile.Destroy();
            }
        }
    }

    public void Fire()
    {
        var listOfColliders = Physics.OverlapSphere(transform.position, ShieldRange);

        foreach (var potentialTarget in listOfColliders)
        {
            if (IsSameLayer(potentialTarget.gameObject.layer, gameObject.layer)
                && potentialTarget.TryGetComponent<IHealth>(out _userHealth))
            {
                _userHealth.ToggleInvuln(TemporaryInvuln);
                _userHealth.Heal(HealAmount);
                transform.SetParent(potentialTarget.transform, false);
                transform.localScale = Vector3.one;
                transform.localPosition = Vector3.zero;
                break;
            }
        }
    }

    public void DoDamage(IHealth enemyHealth)
    {
        // Doesn't do damage
    }

    public void OnCollisionEnter(Collision collision)
    {
        // Doesn't collide
    }

    public void SetLayer(int layer)
    {
        gameObject.layer = layer;
        var children = GetComponentsInChildren<Transform>(includeInactive: true);
        foreach (var child in children)
        {
            child.gameObject.layer = layer;
        }
    }

    public void Destroy()
    {
        _userHealth?.ToggleInvuln(false);
        ObjectPoolManager.ReturnToPool(gameObject);
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, ShieldRange);
    }

    private bool IsSameLayer(int targetLayer, int sourceLayer)
    {
        if (targetLayer == gameObject.layer)
        {
            return true;
        }
        else if (targetLayer == LayerMask.NameToLayer("Player") && gameObject.layer == LayerMask.NameToLayer("PlayerBullet"))
        {
            return true;
        }
        else if (targetLayer == LayerMask.NameToLayer("Enemy") && gameObject.layer == LayerMask.NameToLayer("EnemyBullet"))
        {
            return true;
        }

        return false;
    }
}
