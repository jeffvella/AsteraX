using UnityEngine;


public class Asteroid : MonoBehaviour, IPoolable<Asteroid>, IEdgeWrappable
{
    public float MovementSpeed;
    public Vector3 MoveDirection;
    public Quaternion AngularVelocity;
    public AsteroidType Type;
    private IObjectPool<Asteroid> _pool;
    private Collider _collider;
    private Rigidbody _rigidBody;
    private bool _isSpawned;
    private bool _isDestroyed;

    public Bounds Bounds => _collider.bounds;

    bool IEdgeWrappable.HasWrapped { get; set; }

    void Update()
    {
        if (Game.Time.IsPaused)
             return;

        //transform.position += MoveDirection * MovementSpeed * Time.deltaTime;
        //transform.rotation = AngularVelocity * transform.rotation;

        _rigidBody.MoveRotation(AngularVelocity * _rigidBody.rotation);
        _rigidBody.MovePosition(_rigidBody.position + MoveDirection * MovementSpeed * Time.deltaTime);

        Debug.DrawLine(transform.position, transform.position + MoveDirection.normalized * 3f);
    }

    public void Awake()
    {
        _collider = GetComponent<Collider>();
        _rigidBody = GetComponent<Rigidbody>();
    }

    public void OnSpawned(IObjectPool<Asteroid> pool)
    {
        _pool = pool;
        _isDestroyed = false;
    }

    public void OnDespawned()
    {

    }

    public void Despawn()
    {
        _pool.Despawn(this);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (_isDestroyed)
            return;

        var bullet = collision.transform.GetComponent<Bullet>();
        if (bullet != null && bullet.IsValid)
        {
            _isDestroyed = true;

            // The bullet needs to be disabled before spawning child asteroids
            // or any children spawned will immediately collide with it.
            bullet.Despawn();
            Despawn();

            Game.Events.BulletAsteroidCollision.Raise((this, bullet));
        }
    }


}
