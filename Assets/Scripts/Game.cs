using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This is the game controller and central access point for communications between managers (and is to be
/// abstracted later with interfaces where necessary). There should be no communication between code outside
/// of this path-way to make dependencies clear.
/// </summary>
public class Game : MonoBehaviour
{
    [SerializeField]
    private GameData _gameData;
    private static Game _instance;

    // The fields throughout the project are used instead of auto-property
    // with private getter for the purpose of exposing members in Unity's debug mode inspector.

    private AsteroidManager _asteroidManager;
    private PlayerManager _playerManager;
    private BulletManager _bulletManager;
    private WrapManager _wrapManager;

    // The multiple public statics through are for the convenience of
    // shorter access e.g. Game.Player versus Game.Instance.Player.

    public static AsteroidManager Asteroids => _instance._asteroidManager;

    public static PlayerManager Player => _instance._playerManager;

    public static BulletManager Bullets => _instance._bulletManager;

    public static WrapManager Wrap => _instance._wrapManager;

    public static GameData GameData => _instance._gameData;

    private Game()
    {
        _instance = this;
    }

    public void Awake()
    {
        _wrapManager = Instantiate(_gameData.Managers.WrapManager, parent: transform);
        _asteroidManager = Instantiate(_gameData.Managers.AsteroidManagerPrefab, parent: transform);
        _playerManager = Instantiate(_gameData.Managers.PlayerManagerPrefab, parent: transform);
        _bulletManager = Instantiate(_gameData.Managers.BulletManagerPrefab, parent: transform);
    }

    public static void StartGame()
    {
        SpawnAsteroids();
        SpawnPlayer();
    }

    public static void SpawnAsteroids()
    {
        for (int i = 0; i < GameData.StartingAsteroids; i++)
        {
            Asteroids.SpawnAsteroid();
        }
    }

    private static ShipController SpawnPlayer()
    {
        var ship = Player.SpawnShip();
        return ship;
    }


}