using System;
using System.Collections;
using System.Collections.Generic;
using Events;
using UnityEditor;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.Experimental.XR;
using UnityEngine.UIElements;
using UnityStandardAssets.CrossPlatformInput;

public enum ShipState
{
    None = 0,
    Awake,
    Alive,
    Dead,
}

public class ShipController : MonoBehaviour
{
    [Serializable]
    public struct ShipStatus
    {
        public float Health;
        public ShipState State;
    }

    [Header("Setup")]
    public Transform TurretRotator;
    public Transform TiltTransform;

    public ShipStatus Status => _status;
    private ShipStatus _status;
    private Vector3 _direction;
    private Collider _collider;
    private Rigidbody _rigidBody;

    public ShipData ShipData { get; set; }

    public bool IsStateTransitioning { get; private set; }

    public Bounds Bounds => _collider.bounds;

    void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        SetState(ShipState.Awake);
    }

    void Update()
    {        
        RotateTurretToMousePosition();
        FireWhenButtonDown();
        TiltForwardsWhenMoving();
    }

    private void FireWhenButtonDown()
    {
        if (CrossPlatformInputManager.GetButtonUp("Fire1"))
        {
            Game.Bullets.SpawnBullet(transform.position, TurretRotator.rotation);
        }
    }

    private void MoveShipFromInput()
    {
        var leftRight = CrossPlatformInputManager.GetAxis("Horizontal");
        var upDown = CrossPlatformInputManager.GetAxis("Vertical");

        // Camera view is from top down, ship moves on XY plane.
        // clamp limits excess velocity on diagonal axes.
        _direction = Vector3.ClampMagnitude(new Vector3(leftRight, 0, upDown),1);

        transform.position += _direction * ShipData.MaxSpeed * Time.deltaTime;
        //_rigidBody.MovePosition(_rigidBody.position + _direction * ShipData.MaxSpeed * Time.deltaTime);
    }

    private void TiltForwardsWhenMoving()
    {
        var axis = Vector3.Cross(_direction.normalized, -transform.up);
        var tilt = Quaternion.AngleAxis(ShipData.TiltDegrees, axis);
        TiltTransform.rotation = Quaternion.Slerp(transform.rotation, tilt, _direction.magnitude);
    }

    /// <summary>
    /// Rotates the turret so that it always faces the mouse cursor or last tap position on mobile.
    /// </summary>
    private void RotateTurretToMousePosition()
    {
        var mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        TurretRotator.LookAt(new Vector3(mouseWorldPosition.x, transform.position.y, mouseWorldPosition.z));
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.GetComponent<Asteroid>() is Asteroid asteroid)
        {
            ApplyDamage(asteroid.Type.CollisionDamage); 
        }        
    }

    public void ApplyDamage(float amount)
    {
        _status.Health = Math.Max(0, _status.Health - amount);

        if (_status.Health <= 0)
        {
            SetState(ShipState.Dead);
        }
    }

    private void FixedUpdate()
    {
        MoveShipFromInput();
    }

    public void SetState(ShipState state)
    {
        if (IsStateTransitioning)
            throw new InvalidOperationException("Ship state changed while already changing state");

        IsStateTransitioning = true;
        switch (state)
        {
            case ShipState.Awake: break;
            case ShipState.None: break;

            case ShipState.Alive:
                SpawnSequence();
                break;        
                
            case ShipState.Dead:
                DeathSequence();
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
        _status.State = state;
        IsStateTransitioning = false;
    }

    private void SpawnSequence()
    {
        _status.Health = ShipData.StartingHealth;
    }

    private void DeathSequence()
    {
        Destroy(gameObject);

        ShipData.DestroyedEvent.Raise(new ShipStatusEventArgument(GetInstanceID(), _status));
    }
}

