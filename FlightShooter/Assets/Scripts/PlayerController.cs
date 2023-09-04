using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float MaxThrust;

    public float LiftStrength;
    public float YawStrength;
    public float InducedDragStrength;

    public Vector3 TurnSpeed;
    public Vector3 TurnAcceleration;

    private Rigidbody _rb;
    private Vector3 _localVelocity;
    private Vector3 _localAngularVelocity;
    private float _angleOfAttack;
    private float _angleOfYaw;

    private void Awake()
    {
        if (TryGetComponent<Rigidbody>(out _rb) == false)
        {
            throw new MissingComponentException("Player Plane is missing a RigidBody component");
        }

        _rb.drag = Mathf.Epsilon;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateCurrentState();

        AddThrust();
        AddSimulatedDrag();
        AddLift();

        //Steer(Time.fixedDeltaTime);
    }

    private void UpdateCurrentState()
    {
        _localVelocity = transform.InverseTransformDirection(_rb.velocity);
        _localAngularVelocity = transform.InverseTransformDirection(_rb.angularVelocity);

        _angleOfAttack = Mathf.Atan2(-_localVelocity.y, _localVelocity.z);
        _angleOfYaw = Mathf.Atan2(_localVelocity.x, _localVelocity.z);
    }

    private void AddThrust()
    {
        _rb.AddRelativeForce(MaxThrust * Vector3.forward);
    }

    private void AddSimulatedDrag()
    {
        var velocitySqr = _localVelocity.sqrMagnitude;
        var coefficient = (2 * _localVelocity.normalized);

        var drag = coefficient.magnitude * velocitySqr * (-1 * _localVelocity.normalized);

        _rb.AddRelativeForce(drag);
    }

    private void AddLift()
    {
        if (_localVelocity.sqrMagnitude < 1f)
        {
            return;
        }

        var lift = CalculateLiftForce(_angleOfAttack, Vector3.right, LiftStrength, 30f);
        var yaw = CalculateLiftForce(_angleOfYaw, Vector3.up, YawStrength, 30f);

        _rb.AddRelativeForce(lift);
        _rb.AddRelativeForce(yaw);
    }

    private Vector3 CalculateLiftForce(float angleOfAttack, Vector3 perpendicular, float liftStrength, float attackApex)
    {
        var liftVel = Vector3.ProjectOnPlane(_localVelocity, perpendicular);
        var velocitySqr = liftVel.sqrMagnitude;
        var liftCoefficient = (angleOfAttack * Mathf.Rad2Deg) / attackApex;
        var liftForce = velocitySqr * liftCoefficient * liftStrength;

        var liftDir = Vector3.Cross(liftVel.normalized, perpendicular);
        var lift = liftDir * liftForce;

        var baseDrag = Mathf.Pow(liftCoefficient, 2) * InducedDragStrength;
        var dragDir = liftVel.normalized * -1;
        var drag = dragDir * baseDrag * velocitySqr;

        return lift + drag;
    }

    private void Steer(float dt)
    {
        Vector3 dummy = new Vector3(0, 0, 0);

        var targetAngularVelocity = Vector3.Scale(dummy, TurnSpeed);
        // TO REDO
        var av = _localAngularVelocity * Mathf.Rad2Deg;

        var correction = new Vector3(
            CalculateSteering(dt, av.x, targetAngularVelocity.x, TurnAcceleration.x),
            CalculateSteering(dt, av.y, targetAngularVelocity.y, TurnAcceleration.y),
            CalculateSteering(dt, av.z, targetAngularVelocity.z, TurnAcceleration.z)
        );

        _rb.AddRelativeTorque(correction * Mathf.Rad2Deg, ForceMode.VelocityChange);
    }

    private float CalculateSteering(float dt, float angularVelocity, float targetVelocity, float acceleration)
    {
        var error = targetVelocity - angularVelocity;
        var accel = acceleration * dt;
        return Mathf.Clamp(error, -accel, accel);
    }
}
