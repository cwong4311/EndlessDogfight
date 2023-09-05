using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float MaxThrust;

    public float LiftStrength;
    public float YawStrength;
    public float InducedDragStrength;

    public float TurnSpeed;

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

        HandlePlayerSteer();
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
        var coefficient =  _localVelocity.normalized;

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

    private void HandlePlayerSteer()
    {
        var yaw = Input.GetAxis("Horizontal");
        var pitch = Input.GetAxis("Vertical");
        var roll = Input.GetAxis("Roll");

        Vector3 rollForce = -1 * transform.forward * roll * TurnSpeed;
        Vector3 pitchForce = -1 * transform.right * pitch * TurnSpeed;
        Vector3 yawForce = transform.up * yaw * TurnSpeed;

        _rb.AddTorque(rollForce + pitchForce + yawForce);
    }
}
