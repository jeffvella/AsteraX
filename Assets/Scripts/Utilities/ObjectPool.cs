using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

/// <summary>
/// An item that can be pooled.
/// </summary>
/// <typeparam name="T"></typeparam>
public interface IPoolable<out T> where T : Component, IPoolable<T>

{
    /// <summary>
    /// Event that fires when an object is taken out of the pool and made active.
    /// </summary>
    void OnSpawned(IObjectPool<T> pool);

    /// <summary>
    /// Event that fires when an object is returned to the pool and made inactive.
    /// </summary>
    void OnDespawned();
}

/// <summary>
/// The basic pool functionality that can be accessed by a pooled item.
/// </summary>
/// <typeparam name="T">The component type to be pooled</typeparam>
public interface IObjectPool<in T> where T : Component, IPoolable<T>
{
    /// <summary>
    /// Return an object back to the pool to be re-used.
    /// </summary>
    void Despawn(T item);
}

/// <summary>
/// Functionality for a class to watch a pool and be informed of items being created/spawned/despawned;
/// </summary>
/// <typeparam name="T">The component type to be pooled</typeparam>
public interface IPoolObserver<T> where T : Component, IPoolable<T>
{
    /// <summary>
    /// Called when an item is first initialized in a pool (after which, it becomes available to be spawned)
    /// </summary>
    /// <param name="pool">The pool that created the item</param>
    /// <param name="item">the created item</param>
    void OnItemCreated(IObjectPool<T> pool, T item);

    /// <summary>
    /// Called when an item is taken from the pool
    /// </summary>
    void OnItemSpawned(IObjectPool<T> pool, T item);

    /// <summary>
    /// Called when an item is returned to the pool.
    /// </summary>
    void OnItemDespawned(IObjectPool<T> pool, T item);
}

/// <summary>
/// A pool for game objects; allows objects to be re-used for better performance.
/// </summary>
public class ObjectPool<T> : IObjectPool<T> where T : Component, IPoolable<T>
{
    private readonly Queue<T> _available = new Queue<T>();
    private readonly Dictionary<int, T> _active = new Dictionary<int, T>();
    private readonly GameObject _prefab;
    private readonly GameObject _parent;
    private readonly Action<T> _initializer;
    private readonly IPoolObserver<T> _observer;

    public int Count => AvailableCount + ActiveCount;
    public int AvailableCount => _available.Count;
    public int ActiveCount => _active.Count;

    public T[] ToActiveArray() => _active.Values.ToArray();

    public ObjectPool(GameObject prefab, GameObject parent, int startingSize, IPoolObserver<T> observer = null)
    {
        _prefab = prefab;
        _parent = parent;
        _observer = observer;
        Expand(startingSize);
    }


    /// <summary>
    /// Warms up the pool, creating many items in advance so that they can be spawned later without initialization overheads.
    /// </summary>
    /// <param name="newItemCount">how many items to create</param>
    public void Expand(int newItemCount = default)
    {
        if (newItemCount <= 0)
        {
            newItemCount = DefaultExpandAmount();
        }

        for (int i = 0; i < newItemCount; i++)
        {
            GameObject go = Object.Instantiate(_prefab, Vector3.zero, Quaternion.identity);
            go.SetActive(false);
            go.transform.parent = _parent.transform;
            var t = go.GetComponent<T>();
            if (t == null)
            {
                t = go.AddComponent<T>();
            }
            _available.Enqueue(t);
            _observer?.OnItemCreated(this, t);
        }
    }

    private int DefaultExpandAmount()
    {
        return Math.Max(5, _active.Count / 2);
    }

    /// <summary>
    /// Instantiates a pooled GameObject
    /// </summary>
    /// <param name="position">The starting position</param>
    /// <param name="rotation">The starting rotation</param>
    /// <param name="scale">The starting scale</param>
    /// <returns>an instantiated game object</returns>
    public T Spawn(Vector3 position, Quaternion rotation, Vector3 scale = default)
    {
        if (_available.Count == 0)
        {
            Expand();
        }

        var t = _available.Dequeue();
        t.gameObject.transform.position = position;
        t.gameObject.transform.rotation = rotation;

        if (scale != default)
        {
            t.gameObject.transform.localScale = scale;
        }

        t.gameObject.SetActive(true);
        _active[t.GetInstanceID()] = t;
        t.OnSpawned(this);
        _observer?.OnItemSpawned(this, t);
        return t;
    }

    /// <summary>
    /// Destroy a pooled GameObject
    /// </summary>
    /// <param name="item">item to be destroyed</param>
    public void Despawn(T item) 
    {
        if (item is T t)
        {
            t.gameObject.SetActive(false);
            t.OnDespawned();
            _active.Remove(t.GetInstanceID());
            _available.Enqueue(t);
            _observer?.OnItemDespawned(this, t);
        }
        else
        {
            throw new ArgumentException($"Cannot despawn object because the pool is for a different type; expected: '{typeof(T).Name}'");
        }
    }

}


