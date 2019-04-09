using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Experimental.VFX;
using Object = UnityEngine.Object;
using Random = System.Random;

/// <summary>
/// Responsible for spawning and keeping track of asteroids.
/// </summary>
public class AsteroidManager : MonoBehaviour, IPoolObserver<Asteroid>
{
    [Header("Setup")]
    public GameObject ParentContainer;
    public AsteroidData AsteroidData;
    private Dictionary<int, ObjectPool<Asteroid>> _pools;
    public int StartingPoolSize = 10;

    void Awake()
    {
        _pools = new Dictionary<int, ObjectPool<Asteroid>>();
    }

    public Asteroid SpawnAsteroid(int maxSize = int.MaxValue, Vector3 position = default, Vector3 velocity = default, Quaternion rotation = default)
    {
        var prefab = GetRandomAsteroidPrefab();
        var pool = GetPoolForPrefab(prefab);

        var pos = GetRandomPosition(position);
        var rot = UnityEngine.Random.rotation;
        var asteroid = pool.Spawn(prefab, pos, rot);

        var type = GetRandomAsteroidType(maxSize);
        asteroid.Type = type;
        asteroid.transform.localScale = Vector3.one * type.Size;
        asteroid.MovementSpeed = GetAsteroidMovementSpeed() / type.Size;
        asteroid.AngularVelocity = CreateAsteroidRotationalVelocity();
        asteroid.MoveDirection = GetAsteroidMoveDirection(velocity).Forward;

        GameplayArea.TryMoveInsideBounds(asteroid.gameObject);

        return asteroid;
    }

    public Asteroid SpawnAsteroid(Asteroid parent)
    {
        return SpawnAsteroid(parent.Type.Size-1, parent.transform.position, parent.MoveDirection);
    }

    public const float OnSplitInheritParentMomentumFraction = 0.35f;

    private (Vector3 Forward, Vector3 Up) GetAsteroidMoveDirection(Vector3 parentVelocity = default)
    {
        Quaternion yAxisRotation = Quaternion.AngleAxis(UnityEngine.Random.Range(0,360), Vector3.up);

        // Point children vaguely in the same direction of their parent.
        var rot = parentVelocity != default 
            ? Quaternion.Slerp(Quaternion.LookRotation(parentVelocity), yAxisRotation, OnSplitInheritParentMomentumFraction) 
            : yAxisRotation;

        Vector3 forward = rot * Vector3.forward;
        Vector3 up = rot * Vector3.up;
        return (forward.normalized, up.normalized);
    }

    private ObjectPool<Asteroid> GetPoolForPrefab(GameObject prefab)
    {
        var prefabId = prefab.GetInstanceID();
        if (!_pools.ContainsKey(prefabId))
        {
            var pool = new ObjectPool<Asteroid>(prefab, ParentContainer, StartingPoolSize, this);
            _pools[prefabId] = pool;
            return pool;
        }
        return _pools[prefabId];
    }

    private GameObject GetRandomAsteroidPrefab()
    {
        var randomIndex = UnityEngine.Random.Range(0, AsteroidData.AsteroidPrefabs.Count);
        var prefab = AsteroidData.AsteroidPrefabs[randomIndex];
        return prefab;
    }

    private AsteroidType GetRandomAsteroidType(int maxSize)
    {
        AsteroidType[] types = AsteroidData.AsteroidTypes.Where(t => t.Size <= maxSize).ToArray();
        var randomIndex = UnityEngine.Random.Range(0, types.Length);
        if (randomIndex < 0 || randomIndex >= types.Length)
        {
            Debug.Log($"Invalid range {randomIndex} range=0-{types.Length-1}");
        }
        var type = types.ElementAtOrDefault(randomIndex);
        return type;
    }

    private Vector3 GetRandomPosition(Vector3 position)
    {
        if (position == Vector3.zero)
        {
            position = GameplayArea.RandomPointInBounds();
        }
        position.y = GameplayArea.GetBounds().center.y;
        return position;
    }

    private Quaternion CreateAsteroidRotationalVelocity()
    {
        var x = UnityEngine.Random.Range(AsteroidData.MinRotationSpeed, AsteroidData.MaxRotationSpeed);
        var y = UnityEngine.Random.Range(AsteroidData.MinRotationSpeed, AsteroidData.MaxRotationSpeed);
        var z = UnityEngine.Random.Range(AsteroidData.MinRotationSpeed, AsteroidData.MaxRotationSpeed);
        return Quaternion.Euler(x, y, z);      
    }

    private float GetAsteroidMovementSpeed()
    {
        return UnityEngine.Random.Range(AsteroidData.MinMoveSpeed, AsteroidData.MaxMoveSpeed);
    }


    void IPoolObserver<Asteroid>.OnItemCreated(IObjectPool<Asteroid> pool, Asteroid asteroid)
    {

    }

    void IPoolObserver<Asteroid>.OnItemSpawned(IObjectPool<Asteroid> pool, Asteroid asteroid)
    {

    }

    void IPoolObserver<Asteroid>.OnItemDespawned(IObjectPool<Asteroid> pool, Asteroid asteroid)
    {
        if (asteroid.Type.Size > 1)
        {
            for (int i = 0; i < asteroid.Type.Children; i++)
            {
                SpawnAsteroid(asteroid);
            }
        }
    }

}



