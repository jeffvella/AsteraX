using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour, IPoolable<Asteroid>
{
    public float MovementSpeed;
    public Vector3 MoveDirection;
    public Quaternion AngularVelocity;
    public AsteroidType Type;
    private IObjectPool<Asteroid> _pool;

    void Update()
    {
        transform.position += MoveDirection * MovementSpeed * Time.deltaTime;
        transform.rotation = AngularVelocity * transform.rotation;

        Debug.DrawLine(transform.position, transform.position + MoveDirection.normalized * 3f);
    }

    public void OnSpawned(IObjectPool<Asteroid> pool)
    {
        _pool = pool;
    }

    public void OnDespawned()
    {

    }

    public void Despawn()
    {
        _pool.Despawn(this);
    }

    public void OnCollisionEnter(Collision col)
    {
        var bullet = col.transform.GetComponent<Bullet>();
        if (bullet != null && bullet.IsValid)
        {
            // The bullet needs to be disabled before spawning child asteroids
            // or any children spawned will immediately collide with it.
            bullet.Despawn();
            Despawn();
        }
    }
}
