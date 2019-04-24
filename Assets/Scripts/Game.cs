using System;
using System.Collections;
using Assets.Scripts.Managers;
using UnityEditorInternal;
using UnityEngine;

public enum GameState
{
    None = 0,
    Initialized,
    Loading,
    GameLoaded,
    LevelStarted,
    GameOver,
    LevelComplete,
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
    private LevelManager _levelManager;
    private TimeManager _timeManager;

    // The public statics to instance are for the convenience of shorter access
    // e.g. Game.Player versus Game.Instance.Player.

    public static AsteroidManager Asteroids => _instance._asteroidManager;

    public static PlayerManager Player => _instance._playerManager;

    public static BulletManager Bullets => _instance._bulletManager;

    public static WrapManager Wrap => _instance._wrapManager;

    public static InterfaceManager Interface => _instance._interfaceManager;

    public static EffectsManager Effects => _instance._effectsManager;

    public static LevelManager Levels => _instance._levelManager;

    public static GameData GameData => _instance._gameData;

    public static EventReferences Events => _instance._gameData.Events;

    public static GameState State => _instance._state;

    public static TimeManager Time => _instance._timeManager;


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
        _levelManager = Instantiate(_gameData.Managers.LevelManagerPrefab, parent: transform);
        _timeManager = new TimeManager();

        Events.SessionUpdated.Register(OnSessionUpdated);
        Events.BulletAsteroidCollision.Register(OnAsteroidDestroyed);
    }

    public void Start()
    {
        SetState(GameState.GameLoaded);
    }

    public static void StartGame()
    {
        Levels.FirstLevel();
    }

    private void OnSessionUpdated(PlayerManager.PlayerSession session)
    {
        if (_state == GameState.LevelStarted && session.Lives <= 0)
        {
            StartCoroutine(GameOverSequence());
        }
    }
    private IEnumerator GameOverSequence()
    {
        yield return new WaitForSeconds(1);
        SetState(GameState.GameOver);
    }

    private void OnAsteroidDestroyed((Asteroid Asteroid, Bullet Bullet) obj)
    {
        if (_state == GameState.LevelStarted && Asteroids.ActiveCount == 0)
        {
            StartCoroutine(LevelCompleteSequence());
        }
    }

    private IEnumerator LevelCompleteSequence()
    {
        yield return new WaitForSeconds(1);
        SetState(GameState.LevelComplete);
    }

    public static void SetState(GameState newState)
    {
        var previous = _instance._state;

        switch (newState)
        {
            case GameState.None: break;
            case GameState.Initialized: break;
            case GameState.Loading: break;
            case GameState.GameLoaded: break;

            case GameState.LevelComplete:
                ResetGame();
                break;

            case GameState.LevelStarted:
                SpawnAsteroids();
                SpawnPlayer();
                break;

            case GameState.GameOver:
                ResetGame();
                break;

            default:
                throw new GameExceptions.InvalidStateChangeException<GameState>(previous, newState);
        }

        _instance._state = newState;
        Events.GameStateChanged.Raise((previous, newState));
    }

    public static void ResetGame()
    {
        Player.Clear();
        Asteroids.Clear();
        Bullets.Clear();
        Effects.Clear();
    }

    public static void SpawnAsteroids()
    {
        for (int i = 0; i < Levels.CurrentLevel.StartingAsteroids; i++)
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
