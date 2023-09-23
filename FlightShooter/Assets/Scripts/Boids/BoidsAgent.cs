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
    public float TurnDampingFactor = 5f;
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
        // RB Physics method
        if (_rb != null)
        {
            float angleDifference = Vector3.Angle(transform.up, desiredDirection);
            Vector3 rotationAxis = Vector3.Cross(transform.up, desiredDirection);

            _rb.AddTorque(rotationAxis * angleDifference * TurnPrecision);
            _rb.AddForce(transform.forward * MovementSpeed, ForceMode.Force);

            _rb.AddTorque(-_rb.angularVelocity * TurnDampingFactor);
        }
        // Transform Update method
        else
        {
            desiredDirection *= TurnPrecision;
            if (desiredDirection.sqrMagnitude > _sqrMaxSpeed)
            {
                desiredDirection = desiredDirection.normalized * MovementSpeed;
            }

            if (desiredDirection.sqrMagnitude < 1f)
            {
                desiredDirection = transform.forward * MovementSpeed;
            }

            transform.forward = desiredDirection;
            transform.position += desiredDirection * Time.deltaTime;
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
