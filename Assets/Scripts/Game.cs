using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.UIElements;
using UnityEngine;
using Events;
using UnityEngine.Experimental.XR;
using UnityEngine.UI;

public enum GameState
{
    None = 0,
    MainMenu,
    Loading,
    GameStarted,
    GameOver,
}

/// <summary>
/// This is the central access point for communications between managers.
/// There should be no communication outside of this path (or events) to make dependencies clear.
/// </summary>
public class Game : MonoBehaviour
{
    [SerializeField]
    private GameData _gameData;
    private static Game _instance;
    private static GameState _state;

    // The fields throughout the project are used instead of auto-property
    // with private getter for the purpose of exposing members in Unity's debug mode inspector.

    private AsteroidManager _asteroidManager;
    private PlayerManager _playerManager;
    private BulletManager _bulletManager;
    private WrapManager _wrapManager;
    private InterfaceManager _interfaceManager;
    private EffectsManager _effectsManager;

    // The public statics to instance are for the convenience of shorter access
    // e.g. Game.Player versus Game.Instance.Player.

    public static AsteroidManager Asteroids => _instance._asteroidManager;

    public static PlayerManager Player => _instance._playerManager;

    public static BulletManager Bullets => _instance._bulletManager;

    public static WrapManager Wrap => _instance._wrapManager;

    public static InterfaceManager Interface => _instance._interfaceManager;

    public static EffectsManager Effects => _instance._effectsManager;

    public static GameData GameData => _instance._gameData;

    public static EventReferences Events => _instance._gameData.Events;


    private Game()
    {
        _instance = this;
    }

    private void Awake()
    {
        _interfaceManager = Instantiate(_gameData.Managers.InterfaceManagerPrefab, parent: transform);
        _effectsManager = Instantiate(_gameData.Managers.EffectsManagerPrefab, parent: transform);
        _wrapManager = Instantiate(_gameData.Managers.WrapManagerPrefab, parent: transform);
        _asteroidManager = Instantiate(_gameData.Managers.AsteroidManagerPrefab, parent: transform);
        _playerManager = Instantiate(_gameData.Managers.PlayerManagerPrefab, parent: transform);
        _bulletManager = Instantiate(_gameData.Managers.BulletManagerPrefab, parent: transform);
        
        Events.OnSessionUpdated.Register(OnSessionUpdated);
    }

    private void OnSessionUpdated(PlayerManager.PlayerSession session)
    {
        if (_state == GameState.GameStarted && session.Lives <= 0)
        {
            EndGame();
        }
    }

    public static void StartGame()
    {
        SetState(GameState.GameStarted);
    }

    public static void EndGame()
    {
        if (_state != GameState.GameStarted)
        {
            throw new GameExceptions.InvalidStateChangeException<GameState>(_state, GameState.GameOver);
        }
        SetState(GameState.GameOver);
    }

    private static void SetState(GameState newState)
    {
        var previous = _state;

        switch (newState)
        {
            case GameState.None: break;
            case GameState.MainMenu: break;
            case GameState.Loading: break;

            case GameState.GameStarted:
                SpawnAsteroids();
                SpawnPlayer();
                break;

            case GameState.GameOver:
                ResetGame();
                break;

            default:
                throw new GameExceptions.InvalidStateChangeException<GameState>(previous, newState);
        }

        _state = newState;
        Events.OnGameStateChanged.Raise((previous, newState));
    }

    private static void ResetGame()
    {
        Asteroids.Clear();
        Bullets.Clear();
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

public class GameExceptions
{
    public class InvalidStateChangeException<T> : InvalidOperationException where T : Enum
    {
        public InvalidStateChangeException(T from, T to)
            : base($"{typeof(T)} can't transition from {from} to {to}") { }
    }
}
