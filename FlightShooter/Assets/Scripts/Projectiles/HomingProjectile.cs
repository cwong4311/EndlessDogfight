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

    public float HomingRange;
    public float HomingPrimeDelay;
    public float HomingTrackingSpeed;

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

        Time.timeScale = 0.2f;
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
            _rb.AddForce(1000f * transform.forward, ForceMode.Acceleration);

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
                    if (((1 << potentialTarget.gameObject.layer) & TargetColliders.value) != 0)
                    {
                        _targetEnemy = potentialTarget.transform;
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

        ObjectPoolManager.ReturnToPool(gameObject);
    }

    private void RotateProjectile()
    {
        var direction = _targetEnemy.transform.position - transform.position;
        transform.rotation = Quaternion.LookRotation(-direction, Vector3.up);

        Debug.Log("Missile Rotating: " + _targetEnemy.transform.position + ", " + direction);
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
