using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BoidsAgent : MonoBehaviour
{
    [SerializeField]
    Collider _collider;
    public Collider Collider => _collider;

    public BoidsBehaviour Behaviour;
    public float NeighbourRadius = 60f;
    public float AvoidRadiusMultiplier = 0.5f;

    private float _sqrMaxSpeed;
    private float _sqrNeighbourRadius;
    private float _sqrAvoidanceRadius;
    public float SqrAvoidRadius => _sqrAvoidanceRadius;

    public float MovementSpeed = 10f;
    public float TurnPrecision = 5f;
    private Rigidbody _rb;

    private void Start()
    {
        _sqrMaxSpeed = MovementSpeed * MovementSpeed;
        _sqrNeighbourRadius = NeighbourRadius * NeighbourRadius;
        _sqrAvoidanceRadius = _sqrNeighbourRadius * AvoidRadiusMultiplier * AvoidRadiusMultiplier;

        _rb = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 desiredDirection)
    {
        if (_rb != null)
        {
            _rb.AddRelativeTorque(desiredDirection * TurnPrecision);
            _rb.AddForce(transform.forward * MovementSpeed, ForceMode.Force);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, NeighbourRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, NeighbourRadius * AvoidRadiusMultiplier);
    }
}
