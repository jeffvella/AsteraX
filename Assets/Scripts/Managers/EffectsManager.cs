using System;
using System.Collections;
using System.Linq;
using Assets.Scripts.Utilities;
using Events;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Experimental.VFX;
using Object = UnityEngine.Object;
using Random = System.Random;

/// <summary>
/// Responsible for spawning and re-using effects.
/// </summary>
public class EffectsManager : MonoBehaviour
{
    [Header("Setup")]
    public GameObject ParentContainer;
    private DynamicPool<ParticleEffect> _pool;
    public int StartingPoolSize = 5;

    private void Awake()
    {
        _pool = new DynamicPool<ParticleEffect>
        {
            ParentContainer = ParentContainer,
            StartingPoolSize = StartingPoolSize,
            OnSpawnedAction = OnSpawnedAction
        };
    }

    public ParticleEffect Spawn(GameObject prefab, Vector3 position, Quaternion? rotation = default, Vector3 scale = default)
    {
        var pool = _pool.GetPoolForPrefab(prefab);
        var effect = pool.Spawn(position, rotation ?? Quaternion.identity);
        if (scale != default)
        {
            effect.transform.localScale = scale;
        }
        return effect;
    }

    private void OnSpawnedAction(ParticleEffect effect)
    {
        effect.Restart();        
    }
}