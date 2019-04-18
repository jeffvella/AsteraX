using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ParticleEffect : MonoBehaviour, IPoolable<ParticleEffect>
{
    private IObjectPool<ParticleEffect> _pool;
    private readonly List<ParticleSystem> _systems = new List<ParticleSystem>();

    public int DespawnDelay;
    private bool _isEmissionPaused;
    private bool _isDespawning;

    private void Awake()
    {
        var childSystems = GetComponentsInChildren<ParticleSystem>();
        if (childSystems.Any())
        {
            _systems.AddRange(childSystems);
        }
    }

    public bool IsSpawned => _pool != null;


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
        _isDespawning = false;

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
            _isDespawning = true;

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

    public void Update()
    {
        WrappingEffectFix();
    }

    private void WrappingEffectFix()
    {
        // There's an issue with the trail when warping around the edges
        // where it will get confused and spawn a particle in strange places.
        // This is a temporary fix that is not ideal because the trail breaks
        // on re-entry for a short duration

        if (!Game.Wrap.Contains(transform.position))
        {
            SetEmission(false);
            _isEmissionPaused = true;
        }
        else if (!_isDespawning)
        {
            SetEmission(true);
            _isEmissionPaused = false;
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

