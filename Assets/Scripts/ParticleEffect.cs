using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParticleEffect : MonoBehaviour, IPoolable<ParticleEffect>
{
    private IObjectPool<ParticleEffect> _pool;

    private readonly List<ParticleSystem> _systems = new List<ParticleSystem>();

    private void Awake()
    {
        //var rootSystem = GetComponent<ParticleSystem>();
        //_systems.Add(rootSystem);

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
        _pool?.Despawn(this);
    }
}

