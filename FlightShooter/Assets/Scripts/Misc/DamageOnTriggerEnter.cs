using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DamageOnTriggerEnter : MonoBehaviour
{
    public float Damage;
    public float DelayBeforeApplying;

    public bool AddForceWhenDamaging;
    public float ExplosionRadius;
    public float ExplosionForce;

    public LayerMask TargetCollisionLayer;

    private float _timeSinceEnable;
    private bool _isPrimed;

    private List<int> _alreadyTriggered;
    private List<Collider> _triggeredBeforePrimed;
    
    public void OnEnable()
    {
        _alreadyTriggered = new List<int>();
        _timeSinceEnable = Time.time;

        _isPrimed = false;
        _triggeredBeforePrimed = new List<Collider>();
    }

    public void Update()
    {
        // Prime the source after delay
        if (!_isPrimed && (Time.time - _timeSinceEnable > DelayBeforeApplying))
        {
            _isPrimed = true;

            // Once primed, parse everything collider that is still within range
            var currentColliders = Physics.OverlapSphere(transform.position, ExplosionRadius).ToList();
            foreach (var collider in _triggeredBeforePrimed)
            {
                if (currentColliders.Contains(collider))
                {
                    ApplyDamage(collider);
                }
            }

            _triggeredBeforePrimed.Clear();
        }
    }

    public void OnTriggerEnter(Collider target)
    {
        // If triggered before damage source is primed, store in cache
        if (!_isPrimed)
        {
            _triggeredBeforePrimed.Add(target);
        }
        else
        {
            ApplyDamage(target);
        }
    }

    protected void ApplyDamage(Collider target)
    {
        if (((1 << target.gameObject.layer) & TargetCollisionLayer.value) == 0) return;

        var targetID = target.gameObject.GetInstanceID();
        if (_alreadyTriggered.Contains(targetID) == false)
        {
            _alreadyTriggered.Add(targetID);

            if (target.gameObject.TryGetComponent<IHealth>(out var targetHP))
            {
                targetHP.TakeDamage(Damage);

                if (AddForceWhenDamaging && target.gameObject.TryGetComponent<Rigidbody>(out var targetRB))
                {
                    targetRB.AddExplosionForce(ExplosionForce, transform.position, ExplosionRadius);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, ExplosionRadius);
    }
}
