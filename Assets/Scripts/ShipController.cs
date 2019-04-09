using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class ShipController : MonoBehaviour
{
    [Header("Setup")]
    public Transform TurretRotator;
    public Transform TiltTransform;
    public BulletManager BulletManager;

    [Header("Options")]
    public float Speed = 10f;
    public float TiltDegrees = 30f;
    
    private Vector3 _direction;

    void Awake()
    {

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
            BulletManager.SpawnBullet(transform.position, TurretRotator.rotation);
        }
    }

    private void MoveShipFromInput()
    {
        var horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
        var vertical = CrossPlatformInputManager.GetAxis("Vertical");

        // Camera view is from top down, ship moves on XY plane.
        _direction = Vector3.ClampMagnitude(new Vector3(horizontal, 0, vertical),1);

        // limits excess velocity on diagonal axes.
        transform.position += _direction * Speed * Time.deltaTime;
    }

    private void TiltForwardsWhenMoving()
    {
        var axis = Vector3.Cross(_direction.normalized, -transform.up);
        var tilt = Quaternion.AngleAxis(TiltDegrees, axis);
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
}
