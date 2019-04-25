using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour, IPoolable<Bullet>, IEdgeWrappable
{
    public float Speed { get; set; }

    public float Duration { get; set; }

    public bool HasWrapped { get; set; }

    private IObjectPool<Bullet> _pool;

    private IEnumerator SelfDestructAfterDuration()
    {
        yield return new WaitForSeconds(Duration);
        Despawn();
    }

    void Update()
    {
        if (Game.Time.IsPaused)
            return;

        transform.position += transform.forward * Speed * Time.deltaTime;
    }

    public void OnSpawned(IObjectPool<Bullet> pool)
    {
        _pool = pool;
        StartCoroutine(SelfDestructAfterDuration());

        Game.Events.BulletFired.Raise(this);
    }

    public void OnDespawned()
    {
        _pool = null;
    }

    public bool IsValid => _pool != null;

    public void Despawn()
    {        
        _pool?.Despawn(this);
    }

    public override string ToString()
    {
        return base.ToString();
    }
}

