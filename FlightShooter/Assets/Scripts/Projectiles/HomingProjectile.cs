using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class HomingProjectile : MonoBehaviour, IProjectile
{
    public float Damage { get; set; }

    public float Force { get; set; }

    public float LifeTime { get; set; }

    public Vector3 Direction { get; set; }

    public LayerMask TargetColliders { get; set; }

    public float HomingSpeed;
    public float HomingRange;
    public float HomingPrimeDelay;

    public bool isSnapHoming;
    public float HomingTracking;

    public GameObject OnCollisionEffect;

    private Rigidbody _rb;
    private TrailRenderer _tr;
    private float _aliveSince;

    private Transform _targetEnemy;

    public void OnEnable()
    {
        if (TryGetComponent<Rigidbody>(out _rb) == false)
        {
            throw new MissingComponentException("Bullet does not have a Rigidbody attached");
        }

        _tr = GetComponentInChildren<TrailRenderer>();
        _targetEnemy = null;

        _rb.WakeUp();
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
        if (Time.time - _aliveSince > HomingPrimeDelay)
        {
            _rb.velocity = HomingSpeed * transform.forward;

            if (_targetEnemy != null)
            {
                RotateProjectile();
            }
            else
            {
                var listOfColliders = Physics.OverlapSphere(transform.position, HomingRange).ToList();
                Shuffle(ref listOfColliders);

                foreach (var potentialTarget in listOfColliders)
                {
                    if (((1 << potentialTarget.gameObject.layer) & TargetColliders.value) != 0 
                        && potentialTarget.gameObject.layer != LayerMask.NameToLayer("Environment"))
                    {
                        _targetEnemy = potentialTarget.transform;

                        RotateProjectile();
                        break;
                    }
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
        Debug.Log("Misssile Collided");
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
    }

    public void Destroy()
    {
        _rb.velocity = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _rb.Sleep();

        if (_tr != null) _tr.Clear();

        if (OnCollisionEffect != null)
            Instantiate(OnCollisionEffect, transform.position, Quaternion.identity);

        ObjectPoolManager.ReturnToPool(gameObject);
    }

    private void RotateProjectile()
    {
        var direction = Quaternion.LookRotation(_targetEnemy.transform.position - transform.position, Vector3.up);

        if (isSnapHoming)
        {

            transform.rotation = direction;
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, direction, Time.deltaTime * HomingTracking);
        }
    }

    private void Shuffle<T>(ref List<T> list)
    {
        var random = new System.Random();

        for (int i = list.Count - 1; i > 1; i--)
        {
            int rnd = random.Next(i + 1);

            T value = list[rnd];
            list[rnd] = list[i];
            list[i] = value;
        }
    }

    public void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, transform.forward * 100, Color.red);
    }
}
