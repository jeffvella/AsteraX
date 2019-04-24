using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Responsible for spawning/re-using effects.
/// <para>Requires that particle systems be prefabs with a <see cref="ParticleEffect"/> MonoBehavior.</para>
/// <para>Each <see cref="ParticleSystem"/> should have its 'StopAction' set to Callback and 'PlayOnAwake' enabled</para>
/// </summary>
public class EffectsManager : MonoBehaviour
{
    [Header("Setup")]
    public GameObject ParentContainer;
    private PoolGroup<ParticleEffect> _poolsGroup;
    public int StartingPoolSize = 5;

    private void Awake()
    {
        _poolsGroup = new PoolGroup<ParticleEffect>
        {
            ParentContainer = ParentContainer,
            StartingPoolSize = StartingPoolSize,
            OnSpawnedAction = OnSpawned,
        };
    }

    public ParticleEffect Spawn(ParticleEffect prefab, Transform parentContainer)
    {
        var pool = _poolsGroup.GetPoolForPrefab(prefab.gameObject);
        var effect = pool.Spawn(parentContainer.position, parentContainer.rotation);
        effect.transform.parent = parentContainer;
        return effect;
    }

    public ParticleEffect Spawn(ParticleEffect prefab, Vector3 position, Quaternion? rotation = default, Vector3 scale = default)
    {
        var pool = _poolsGroup.GetPoolForPrefab(prefab.gameObject);
        var effect = pool.Spawn(position, rotation ?? Quaternion.identity);
        if (scale != default)
        {
            effect.transform.localScale = scale;
        }
        return effect;
    }

    public IEnumerator WaitForEffect(ParticleEffect effect, int timeoutSeconds = 10)
    {
        var timeout = DateTime.UtcNow + TimeSpan.FromSeconds(timeoutSeconds);
        while (effect.IsSpawned && effect.IsPlaying)
        {
            if (DateTime.UtcNow > timeout)
                break;

            yield return new WaitForSeconds(0.1f);
        }
        yield return null;
    }

    private void OnSpawned(ParticleEffect effect)
    {
        effect.Restart();        
    }

    public void Clear()
    {
        _poolsGroup.Clear();
    }
}