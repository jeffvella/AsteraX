using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Experimental.VFX;
using Object = UnityEngine.Object;

/// <summary>
/// Responsible for spawning and keeping track of bullets fired from the player ship.
/// </summary>
public class BulletManager : MonoBehaviour, IPoolObserver<Bullet>
{
    [Header("Setup")]
    public GameObject ParentContainer;
    public BulletData BulletData;
    private ObjectPool<Bullet> _pool;

    void Awake()
    {
        _pool = new ObjectPool<Bullet>(BulletData.BulletPrefab, ParentContainer, BulletData.StartingPoolSize, this);
    }

    public Bullet SpawnBullet(Vector3 position, Quaternion rotation)
    {       
        return _pool.Spawn(position, rotation);
    }

    public void OnItemCreated(IObjectPool<Bullet> pool, Bullet item)
    {
        item.Speed = BulletData.Speed;
        item.Duration = BulletData.Duration;
    }

    public void OnItemSpawned(IObjectPool<Bullet> pool, Bullet item)
    {
    
    }

    public void OnItemDespawned(IObjectPool<Bullet> pool, Bullet item)
    {
      
    }
}





