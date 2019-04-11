using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

public class ObjectPoolTests
{
    [SetUp]
    public void ResetScene()
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
    }

    [Test]
    public void ObjectPool_SpawnsAttachedToParent()
    {
        var parent = new GameObject();        
        var pool = new ObjectPool<TestComponent>(parent, parent, 1);
        pool.Spawn(Vector3.zero, Quaternion.identity);
        Assert.IsTrue(parent.GetComponentInChildren<TestComponent>() != null);
    }

    [Test]
    public void ObjectPool_InstantiatesStartingSize()
    {
        var poolSize = 10;
        var parent = new GameObject();        
        var pool = new ObjectPool<TestComponent>(parent, parent, poolSize);

        Assert.IsTrue(pool.Count == poolSize);
        
        // todo can't find the objects even though they are being created
        //Assert.IsTrue(Object.FindObjectsOfType<Transform>().Length == poolSize);
    }

    [Test]
    public void ObjectPool_SpawnsItems()
    {
        var numToSpawn = 5;
        var poolSize = 10;

        var parent = new GameObject();
        var pool = new ObjectPool<TestComponent>(parent, parent, poolSize);

        for (int i = 0; i < numToSpawn; i++)
        {
            pool.Spawn(Vector3.zero, Quaternion.identity);
        }

        Assert.IsTrue(pool.AvailableCount == 5);
        Assert.IsTrue(pool.ActiveCount == 5);
    }

    [Test]
    public void ObjectPool_ConstructorRequiresPrefab()
    {
        Assert.Throws<ArgumentException>(() => new ObjectPool<TestComponent>(null, new GameObject(), 1));
    }

    [Test]
    public void ObjectPool_ConstructorRequiresParent()
    {
        Assert.Throws<NullReferenceException>(() => new ObjectPool<TestComponent>(new GameObject(), null, 1));
    }

    [Test]
    public void ObjectPool_EnsuresMinimumPoolSize()
    {
        Assert.DoesNotThrow(() => new ObjectPool<TestComponent>(new GameObject(), new GameObject(), -50));
        Assert.DoesNotThrow(() => new ObjectPool<TestComponent>(new GameObject(), new GameObject(), 0));

        var pool = new ObjectPool<TestComponent>(new GameObject(), new GameObject(), 0);

        Assert.IsTrue(pool.Count > 0);
    }


    public void TestIsNull()
    {
        if (IsDefaultNull(1))
            Console.WriteLine("1 is null? :P");

        if (IsDefaultNull(string.Empty))
            Console.WriteLine("string is null? :P");
    }

    public bool IsDefaultNull<T>(T arg)
    {
        return default(T) == null;
    }

    [Test]
    public void ObjectPool_DespawnsItems()
    {
        TestIsNull();

        var numToSpawn = 5;
        var poolSize = 10;

        var parent = new GameObject();
        var pool = new ObjectPool<TestComponent>(parent, parent, poolSize);

        var spawnedItems = new Queue<TestComponent>();
        for (int i = 0; i < numToSpawn; i++)
        {
            var item = pool.Spawn(Vector3.zero, Quaternion.identity);
            spawnedItems.Enqueue(item);
        }

        for (int i = 0; i < numToSpawn; i++)
        {
            var item = spawnedItems.Dequeue();
            pool.Despawn(item);
        }

        Assert.IsTrue(spawnedItems.Count == 0);
        Assert.IsTrue(pool.ActiveCount == 0);
        Assert.IsTrue(pool.Count == pool.AvailableCount);
    }

    [Test]
    public void ObjectPool_SpawnsWithGivenTransform()
    {
        var poolSize = 10;
        var parent = new GameObject();
        var pool = new ObjectPool<TestComponent>(parent, parent, poolSize);

        var position = new Vector3(10,-50,10000);
        var rotation = Quaternion.LookRotation(Vector3.up);
        var scale = new Vector3(0, 10, 2);

        var item = pool.Spawn(position, rotation, scale);

        Assert.IsTrue(item.transform.position == position);
        Assert.IsTrue(item.transform.rotation == rotation);
        Assert.IsTrue(item.transform.localScale == scale);
    }

    [Test]
    public void ObjectPool_ExpandsPoolWhenRequired()
    {
        var numToSpawn = 15;
        var startingPoolSize = 10;

        var parent = new GameObject();
        var pool = new ObjectPool<TestComponent>(parent, parent, startingPoolSize);

        for (int i = 0; i < numToSpawn; i++)
        {
            pool.Spawn(Vector3.zero, Quaternion.identity);
        }

        Assert.IsTrue(pool.Count >= numToSpawn);
        Assert.IsTrue(pool.Count >= startingPoolSize);
    }

    [Test]
    public void ObjectPool_ObserverReceivesCreatedEvent()
    {
        var poolSize = 5;
        var observer = new TestPoolObserver();
        var parent = new GameObject();
        var pool = new ObjectPool<TestComponent>(parent, parent, poolSize, observer);

        Assert.IsTrue(observer.ItemCreatedCount == poolSize);
        Assert.IsTrue(observer.ItemSpawnedCount == 0);
        Assert.IsTrue(observer.ItemDespawnedCount == 0);
        Assert.IsTrue(observer.LastItemCreated != null);
    }

    [Test]
    public void ObjectPool_ObserverReceivesSpawnedEvent()
    {
        var numToSpawn = 3;
        var poolSize = 5;
        var observer = new TestPoolObserver();
        var parent = new GameObject();
        var pool = new ObjectPool<TestComponent>(parent, parent, poolSize, observer);

        var spawnedItems = new Queue<TestComponent>();
        for (int i = 0; i < numToSpawn; i++)
        {
            var item = pool.Spawn(Vector3.zero, Quaternion.identity);
            spawnedItems.Enqueue(item);

            Assert.IsTrue(observer.LastItemSpawned == item);
            Assert.IsTrue(observer.LastPool == pool);
        }

        Assert.IsTrue(observer.ItemSpawnedCount == numToSpawn);
        Assert.IsTrue(observer.ItemDespawnedCount == 0);
    }

    [Test]
    public void ObjectPool_ObserverReceivesDespawnedEvent()
    {
        var numToSpawn = 3;
        var poolSize = 5;
        var observer = new TestPoolObserver();
        var parent = new GameObject();
        var pool = new ObjectPool<TestComponent>(parent, parent, poolSize, observer);

        var spawnedItems = new Queue<TestComponent>();
        for (int i = 0; i < numToSpawn; i++)
        {
            var item = pool.Spawn(Vector3.zero, Quaternion.identity);
            spawnedItems.Enqueue(item);
        }

        for (int i = 0; i < numToSpawn; i++)
        {
            var item = spawnedItems.Dequeue();
            pool.Despawn(item);

            Assert.IsTrue(observer.LastItemDespawned == item);
            Assert.IsTrue(observer.LastPool == pool);
        }

        Assert.IsTrue(observer.ItemDespawnedCount == numToSpawn);
        Assert.IsTrue(pool.Count >= numToSpawn);
    }

    [Test]
    public void ObjectPool_SpawnedItemCanDespawnItself()
    {
        var poolSize = 5;
        var parent = new GameObject();
        var pool = new ObjectPool<TestComponent>(parent, parent, poolSize);

        var item = pool.Spawn(Vector3.zero, Quaternion.identity);
        item.Despawn();

        Assert.IsTrue(pool.ActiveCount == 0);        
    }

    [Test]
    public void ObjectPool_UnspawnedItemIsInactive()
    {
        var poolSize = 5;
        var observer = new TestPoolObserver();
        var parent = new GameObject();
        var pool = new ObjectPool<TestComponent>(parent, parent, poolSize, observer);

        Assert.IsFalse(observer.LastItemCreated.isActiveAndEnabled);

        var item = pool.Spawn(Vector3.zero, Quaternion.identity);

        Assert.IsTrue(observer.LastItemSpawned.isActiveAndEnabled);

        item.Despawn();

        Assert.IsFalse(observer.LastItemDespawned.isActiveAndEnabled);
    }
}

public class TestPoolObserver : IPoolObserver<TestComponent>
{
    public int ItemCreatedCount;
    public int ItemSpawnedCount;
    public int ItemDespawnedCount;
    public TestComponent LastItemCreated;
    public TestComponent LastItemSpawned;
    public TestComponent LastItemDespawned;
    public IObjectPool<TestComponent> LastPool;

    public void OnItemCreated(IObjectPool<TestComponent> pool, TestComponent item)
    {
        ItemCreatedCount++;
        LastItemCreated = item;
        LastPool = pool;
    }

    public void OnItemSpawned(IObjectPool<TestComponent> pool, TestComponent item)
    {
        ItemSpawnedCount++;
        LastItemSpawned = item;
        LastPool = pool;
    }

    public void OnItemDespawned(IObjectPool<TestComponent> pool, TestComponent item)
    {
        ItemDespawnedCount++;
        LastItemDespawned = item;
        LastPool = pool;
    }
}

public class TestComponent : MonoBehaviour, IPoolable<TestComponent>
{
    private IObjectPool<TestComponent> _pool;

    public void OnSpawned(IObjectPool<TestComponent> pool)
    {
        _pool = pool;
    }

    public void OnDespawned()
    {

    }

    public void Despawn()
    {
        _pool.Despawn(this);
    }
}