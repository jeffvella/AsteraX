using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParticleEffect : MonoBehaviour, IPoolable<ParticleEffect>
{
    private IObjectPool<ParticleEffect> _pool;
    private readonly List<ParticleSystem> _systems = new List<ParticleSystem>();

    public int DespawnDelay;

    private void Awake()
    {
        var childSystems = GetComponentsInChildren<ParticleSystem>();
        if (childSystems.Any())
        {
            _systems.AddRange(childSystems);
        }
    }

    public void OnParticleSystemStopped()
    {
        Despawn();
    }

    public void OnSpawned(IObjectPool<ParticleEffect> pool)
    {
        _pool = pool;
    }

    public void OnDespawned()
    {
        _pool = null;
    }

    public void Reset()
    {
        for (int i = 0; i < _systems.Count; i++)
        {
            var system = _systems[i];
            system.time = 0;
            system.Clear();
            SetEmission(system, true);
        }
    }
    private void Play()
    {
        for (int i = 0; i < _systems.Count; i++)
        {
            _systems[i].Play();
        }
    }

    private void Stop()
    {
        for (int i = 0; i < _systems.Count; i++)
        {
            _systems[i].Stop();
        }
    }

    private void Pause()
    {
        for (int i = 0; i < _systems.Count; i++)
        {
            _systems[i].Pause();
        }
    }

    public void Restart()
    {
        Reset();
        Play();
    }

    public bool IsValid => _pool != null;

    public void Despawn()
    {
        if (DespawnDelay > 0)
        {
            // Prevent particles being destroyed if parent becomes inactive
            transform.parent = _pool.ParentContainer;

            SetEmission(false);
            StartCoroutine(DespawnAfterDelay(DespawnDelay));
        }
        else
        {
            _pool?.Despawn(this);
        }
    }

    private void SetEmission(bool value)
    {
        for (int i = 0; i < _systems.Count; i++)
        {
            SetEmission(_systems[i], value);
        }
    }

    public void SetEmission(ParticleSystem system, bool value)
    {
        var emission = system.emission;
        emission.enabled = value;
    }

    public IEnumerator DespawnAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        _pool?.Despawn(this);
    }


}

