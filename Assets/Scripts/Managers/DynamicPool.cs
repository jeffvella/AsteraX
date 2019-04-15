using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DynamicPool<T> : IPoolObserver<T> where T : Component, IPoolable<T>
{
    private readonly Dictionary<int, ObjectPool<T>> _pools;

    public DynamicPool()
    {
        _pools = new Dictionary<int, ObjectPool<T>>();
    }

    public DynamicPool(GameObject parent, int startingPoolSize) : this()
    {
        ParentContainer = parent;
        StartingPoolSize = startingPoolSize;
    }

    public GameObject ParentContainer { get; set; }

    public int StartingPoolSize { get; set; }

    public Action<T> OnCreatedAction { get; set; }

    public Action<T> OnSpawnedAction { get; set; }

    public Action<T> OnDepsawnedAction { get; set; }

    public int ActiveCount => _pools.Sum(p => p.Value.ActiveCount);

    public bool TryGetItem(GameObject prefab, int gameObjectInstanceId, out T item)
    {
        return TryGetItem(prefab.GetInstanceID(), gameObjectInstanceId, out item);
    }

    public bool TryGetItem(int prefabInstanceId, int gameObjectInstanceId, out T item)
    {
        if (!_pools.TryGetValue(prefabInstanceId, out ObjectPool<T> pool) || !pool.ContainsKey(gameObjectInstanceId))
        {
            item = default;
            return false;
        }
        item = pool[gameObjectInstanceId];
        return true;
    }

    public ObjectPool<T> GetPoolForPrefab(GameObject prefab)
    {
        var prefabId = prefab.GetInstanceID();
        if (!_pools.ContainsKey(prefabId))
        {
            var pool = new ObjectPool<T>(prefab, ParentContainer, StartingPoolSize, this);
            _pools[prefabId] = pool;
            return pool;
        }
        return _pools[prefabId];
    }

    public void Clear()
    {
        foreach (var pool in _pools)
        {
            foreach (var item in pool.Value.ToActiveArray())
            {
                item.Despawn();
            }
        }
    }

    public void OnItemCreated(IObjectPool<T> pool, T effect)
    {
        OnCreatedAction?.Invoke(effect);
    }

    public void OnItemSpawned(IObjectPool<T> pool, T effect)
    {
        OnSpawnedAction?.Invoke(effect);
    }

    public void OnItemDespawned(IObjectPool<T> pool, T effect)
    {
        OnDepsawnedAction?.Invoke(effect);
    }
}