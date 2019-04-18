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
    Spawning,
    Alive,
    Dying,
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
    public GameObject Model;

    public ShipStatus Status => _status;
    private int _id;
    private ShipStatus _status;
    private Vector3 _direction;
    private Collider _collider;
    private Rigidbody _rigidBody;
    private ParticleEffect _exhaustEmitter;
    private ShipData _shipData;

    public bool IsStateTransitioning { get; private set; }

    public bool IsRendered
    {
        get => Model.activeInHierarchy;
        set => Model.SetActive(value);
    }

    public Bounds Bounds => _collider.bounds;

    void Awake()
    {
        SetState(ShipState.Awake);
    }

    public void Initialize(ShipData data)
    {
        _id = GetInstanceID();
        _rigidBody = GetComponent<Rigidbody>();
        _collider = GetComponent<Collider>();
        _shipData = data;
        _exhaustEmitter = Game.Effects.Spawn(_shipData.ExhaustEffect, transform);
    }

    void Update()
    {
        if (_status.State == ShipState.Alive)
        {
            RotateTurretToMousePosition();
            FireWhenButtonDown();
            TiltForwardsWhenMoving();
        }
    }

    void Start()
    {

    }

    private void FixedUpdate()
    {
        if (_status.State == ShipState.Alive)
        {
            MoveShipFromInput();
        }
    }

    private void FireWhenButtonDown()
    {
        if (CrossPlatformInputManager.GetButtonUp("Fire1"))
        {
            var bullet = Game.Bullets.SpawnBullet(transform.position, TurretRotator.rotation);

            var randomEffectIndex = UnityEngine.Random.Range(0, Game.Bullets.BulletData.BulletEffects.Count);
            var randomEffectDefinition = Game.Bullets.BulletData.BulletEffects[randomEffectIndex];
            var bulletEffect = Game.Effects.Spawn(randomEffectDefinition.Prefab, bullet.transform);

            Game.Bullets.LinkDespawn(bullet, bulletEffect);   
        }
    }

    private bool _isExhaustOn;

    private void MoveShipFromInput()
    {
        var leftRight = CrossPlatformInputManager.GetAxis("Horizontal");
        var upDown = CrossPlatformInputManager.GetAxis("Vertical");

        // Camera view is from top down, ship moves on XY plane.
        // clamp limits excess velocity on diagonal axes.
        _direction = Vector3.ClampMagnitude(new Vector3(leftRight, 0, upDown),1);

        var isMoving = _direction != Vector3.zero;
        if (!isMoving && _exhaustEmitter.IsActive)
        {
            _exhaustEmitter.Stop();
            _isExhaustOn = false;
        }
        else if (isMoving && !_exhaustEmitter.IsActive)
        {
            _exhaustEmitter.Play();
            _isExhaustOn = true;
        }

        transform.position += _direction * _shipData.MaxSpeed * Time.deltaTime;
        //_rigidBody.MovePosition(_rigidBody.position + _direction * ShipData.MaxSpeed * Time.deltaTime);
    }

    private void TiltForwardsWhenMoving()
    {
        var axis = Vector3.Cross(_direction.normalized, -transform.up);
        var tilt = Quaternion.AngleAxis(_shipData.TiltDegrees, axis);
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
        if (_status.State == ShipState.Alive && collision.transform.GetComponent<Asteroid>() is Asteroid asteroid)
        {
            ApplyDamage(asteroid.Type.CollisionDamage); 
        }        
    }

    public void ApplyDamage(float amount)
    {
        _status.Health = Math.Max(0, _status.Health - amount);

        if (_status.Health <= 0)
        {
            SetState(ShipState.Dying);
        }
    }

    public void SetState(ShipState state)
    {
        if (IsStateTransitioning)
        {
            throw new InvalidOperationException("Ship state changed while already changing state");
        }

        IsStateTransitioning = true;
        switch (state)
        {
            case ShipState.Awake:
            case ShipState.None: 
            case ShipState.Alive:
            case ShipState.Dead:
                break;

            case ShipState.Spawning:
                StartCoroutine(SpawnSequence());
                break;

            case ShipState.Dying:
                StartCoroutine(DeathSequence());
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
        _status.State = state;
        IsStateTransitioning = false;
    }

    private IEnumerator SpawnSequence()
    {
        _status.Health = _shipData.StartingHealth;

        var effect = Game.Effects.Spawn(_shipData.SpawnEffect, transform.position);
        yield return Game.Effects.WaitForEffect(effect);

        SetState(ShipState.Alive);
        yield return null;
    }

    private IEnumerator DeathSequence()
    {
        IsRendered = false;

        var effect = Game.Effects.Spawn(_shipData.DespawnEffect, transform.position);
        yield return Game.Effects.WaitForEffect(effect);

        SetState(ShipState.Dead);
        _shipData.DestroyedEvent.Raise(new ShipStatusArgs(_id, _status));

        Destroy(gameObject);
        yield return null;
    }

}

