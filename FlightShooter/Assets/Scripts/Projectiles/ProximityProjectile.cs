using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityProjectile : MonoBehaviour, IProjectile
{
    public float Damage { get; set; }

    public float Force { get; set; }

    public float LifeTime { get; set; }

    public Vector3 Direction { get; set; }

    public LayerMask TargetColliders { get; set; }

    public float ProximityRadius;
    public GameObject CollisionPF;

    private Rigidbody _rb;
    private TrailRenderer _tr;
    private float _aliveSince;

    public void OnEnable()
    {
        if (TryGetComponent<Rigidbody>(out _rb) == false)
        {
            throw new MissingComponentException("Bullet does not have a Rigidbody attached");
        }

        _tr = GetComponentInChildren<TrailRenderer>();

        _rb.WakeUp();
        _aliveSince = Time.time;
    }

    public void Update()
    {
        if (Time.time - _aliveSince >= LifeTime)
        {
            Destroy();
        }

        var proximityTargets = Physics.OverlapSphere(transform.position, ProximityRadius);
        if (proximityTargets != null && proximityTargets.Length > 0)
        {
            foreach (var isApplicableTarget in proximityTargets)
            {
                if (((1 << isApplicableTarget.gameObject.layer) & TargetColliders.value) != 0
                && isApplicableTarget.gameObject.layer != LayerMask.NameToLayer("Environment"))
                {
                    Destroy();
                }
            }
        }
    }

    public void Fire()
    {
        _rb.AddForce(Force * Direction, ForceMode.Force);
        _rb.angularVelocity = Vector3.zero;
    }

    public void DoDamage(IHealth enemyHealth)
    {
        enemyHealth.TakeDamage(Damage);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & TargetColliders.value) != 0)
        {
            if (collision.gameObject.TryGetComponent<IHealth>(out var enemyHealth))
            {
                DoDamage(enemyHealth);
            }
            
            Destroy();
        }
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
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _rb.Sleep();

        if (_tr != null) _tr.Clear();

        if (CollisionPF != null)
        {
            Instantiate(CollisionPF, transform.position, Quaternion.identity);
            if (CollisionPF.TryGetComponent<DamageOnTriggerEnter>(out var collisionDamager))
            {
                collisionDamager.TargetCollisionLayer = TargetColliders;
            }
        }

        ObjectPoolManager.ReturnToPool(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, ProximityRadius);
    }
}