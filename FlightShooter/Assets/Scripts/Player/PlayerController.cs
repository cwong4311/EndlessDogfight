using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float MaxThrust;

    public float LiftStrength;
    public float YawStrength;
    public float InducedDragStrength;

    public float TurnSpeed;
    public float RollSpeed;

    public float CollisionAvoidanceSpeed;
    public LayerMask CollisionAvoidanceLayerMask;

    public WeaponsMananger WeaponsMananger;

    private Rigidbody _rb;
    private Vector3 _localVelocity;
    private Vector3 _localAngularVelocity;
    private float _angleOfAttack;
    private float _angleOfYaw;
    private float _currentVerticalTurnSpeed;
    private float _currentHorizontalTurnSpeed;

    public bool CanShoot = true;

    private float _pitch;
    private float _yaw;
    private float _roll;
    private bool _primaryFiring;
    private bool _secondaryFiring;

    private void Awake()
    {
        if (TryGetComponent<Rigidbody>(out _rb) == false)
        {
            throw new MissingComponentException("Player Plane is missing a RigidBody component");
        }

        _rb.drag = Mathf.Epsilon;
        _currentVerticalTurnSpeed = TurnSpeed;
        _currentHorizontalTurnSpeed = TurnSpeed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        UpdateCurrentState();

        AddThrust();
        AddSimulatedDrag();
        AddLift();

        HandlePlayerSteer();
        HandlePlayerShoot();
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
        //var yaw = Input.GetAxis("Horizontal");
        //var pitch = Input.GetAxis("Vertical");
        //var roll = Input.GetAxis("Roll");

        Vector3 rollForce = -1 * transform.forward * _roll * RollSpeed;
        Vector3 pitchForce = -1 * transform.right * _pitch * _currentVerticalTurnSpeed;
        Vector3 yawForce = transform.up * _yaw * _currentHorizontalTurnSpeed;

        _rb.AddTorque(rollForce + pitchForce + yawForce, ForceMode.VelocityChange);
    }

    private void HandlePlayerShoot()
    {
        if (CanShoot)
        {
            if (_primaryFiring)
            {
                WeaponsMananger.ShootPrimary();
            }

            if (_secondaryFiring)
            {
                WeaponsMananger.ShootSecondary();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & CollisionAvoidanceLayerMask.value) != 0)
        {
            _currentVerticalTurnSpeed = CollisionAvoidanceSpeed * 10;
            _currentHorizontalTurnSpeed = CollisionAvoidanceSpeed;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & CollisionAvoidanceLayerMask.value) != 0)
        {
            _currentVerticalTurnSpeed = TurnSpeed;
            _currentHorizontalTurnSpeed = TurnSpeed;
        }
    }

    public void VerticalMove(InputAction.CallbackContext context)
    {
        _pitch = context.ReadValue<float>();
    }

    public void HorizontalMove(InputAction.CallbackContext context)
    {
        _yaw = context.ReadValue<float>();
    }

    public void RollMove(InputAction.CallbackContext context)
    {
        _roll = context.ReadValue<float>();
    }

    public void PrimaryFire(InputAction.CallbackContext context)
    {
        if (context.performed) _primaryFiring = true;
        if (context.canceled) _primaryFiring = false;
    }

    public void SecondaryFire(InputAction.CallbackContext context)
    {
        if (context.performed) _secondaryFiring = true;
        if (context.canceled) _secondaryFiring = false;
    }
}
