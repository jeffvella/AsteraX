using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

[DefaultExecutionOrder(-1)]
public class Game : MonoBehaviour, ISerializationCallbackReceiver, IExposedBehavior
{
    private AsteroidManager _asteroidManager;
    private PlayerManager _playerManager;
    private BulletManager _bulletManager;
    private WrapManager _wrapManager;

    public GameData GameData; 
    private static Game _instance;

    public static AsteroidManager Asteroids => _instance._asteroidManager;
    public static PlayerManager Player => _instance._playerManager;
    public static BulletManager Bullets => _instance._bulletManager;
    public static WrapManager Wrap => _instance._wrapManager;

    private Game()
    {
        _instance = this;
    }

    public void Awake()
    {
        _wrapManager = Instantiate(GameData.Managers.WrapManager, parent: transform);
        _asteroidManager = Instantiate(GameData.Managers.AsteroidManagerPrefab, parent: transform);
        _playerManager = Instantiate(GameData.Managers.PlayerManagerPrefab, parent: transform);
        _bulletManager = Instantiate(GameData.Managers.BulletManagerPrefab, parent: transform);
    }

    public void Update()
    {
        
    }

    public void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        SpawnAsteroids();
        SpawnPlayer();
    }

    public void SpawnAsteroids()
    {
        for (int i = 0; i < GameData.StartingAsteroids; i++)
        {
            _asteroidManager.SpawnAsteroid();
        }
    }

    public void SpawnPlayer()
    {
        var ship = _playerManager.SpawnShip();
    }

    public void OnBeforeSerialize()
    {
        
    }

    public void OnAfterDeserialize()
    {
      
    }


}

public interface IExposedBehavior
{
    void Awake();
    void Update();
}

