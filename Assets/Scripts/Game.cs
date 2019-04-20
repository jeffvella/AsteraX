using System;
using UnityEngine;

public enum GameState
{
    None = 0,
    Initialized,
    Loading,
    Loaded,
    Started,
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
    private GameState _state;

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
        _state = GameState.Initialized;
    }

    private void Awake()
    {
        SetState(GameState.Loading);

        _interfaceManager = Instantiate(_gameData.Managers.InterfaceManagerPrefab, parent: transform);
        _effectsManager = Instantiate(_gameData.Managers.EffectsManagerPrefab, parent: transform);
        _wrapManager = Instantiate(_gameData.Managers.WrapManagerPrefab, parent: transform);
        _asteroidManager = Instantiate(_gameData.Managers.AsteroidManagerPrefab, parent: transform);
        _playerManager = Instantiate(_gameData.Managers.PlayerManagerPrefab, parent: transform);
        _bulletManager = Instantiate(_gameData.Managers.BulletManagerPrefab, parent: transform);
        
        Events.SessionUpdated.Register(OnSessionUpdated);
    }

    public void Start()
    {
        SetState(GameState.Loaded);
    }

    private void OnSessionUpdated(PlayerManager.PlayerSession session)
    {
        if (_state == GameState.Started && session.Lives <= 0)
        {
            EndGame();
        }
    }

    public static void StartGame()
    {
        _instance.SetState(GameState.Started);
    }

    public static void EndGame()
    {
        if (_instance._state != GameState.Started)
        {
            throw new GameExceptions.InvalidStateChangeException<GameState>(_instance._state, GameState.GameOver);
        }
        _instance.SetState(GameState.GameOver);
    }

    private void SetState(GameState newState)
    {
        var previous = _state;

        switch (newState)
        {
            case GameState.None: break;
            case GameState.Initialized: break;
            case GameState.Loading: break;
            case GameState.Loaded: break;

            case GameState.Started:
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
        Events.GameStateChanged.Raise((previous, newState));
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

    private static Ship SpawnPlayer()
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
