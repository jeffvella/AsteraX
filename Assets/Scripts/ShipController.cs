using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Profiling.Memory.Experimental;
using UnityEngine;
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

    public ShipType ShipType { get; set; }

    public bool IsStateTransitioning { get; private set; }

    public Bounds Bounds => _collider.bounds;

    void Awake()
    {
        _collider = GetComponent<Collider>();
        SetState(ShipState.Awake);
    }

    void Update()
    {        
        RotateTurretToMousePosition();
        MoveShipFromInput();
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
        var horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
        var vertical = CrossPlatformInputManager.GetAxis("Vertical");

        // Camera view is from top down, ship moves on XY plane.
        _direction = Vector3.ClampMagnitude(new Vector3(horizontal, 0, vertical),1);

        // limits excess velocity on diagonal axes.
        transform.position += _direction * ShipType.MaxSpeed * Time.deltaTime;
    }

    private void TiltForwardsWhenMoving()
    {
        var axis = Vector3.Cross(_direction.normalized, -transform.up);
        var tilt = Quaternion.AngleAxis(ShipType.TiltDegrees, axis);
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

    public void ApplyDamage(float amount)
    {
        _status.Health = Math.Max(0, _status.Health - amount);

        if (_status.Health <= 0)
        {
            SetState(ShipState.Dead);
        }
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
        _status.Health = ShipType.StartingHealth;
    }

    private void DeathSequence()
    {
        Destroy(gameObject);
    }
}

