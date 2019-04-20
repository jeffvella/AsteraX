using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// The interface manager monitors game events and controls how to represent that information in the interface.
/// </summary>
public class InterfaceManager : MonoBehaviour
{
    [SerializeField]
    private InterfaceData _interfaceData;
    private EventSystem _eventSystem;

    private HudInterface _hud;
    private GameOverInterface _gameOver;
    private MainMenuInterface _mainMenu;

    public void Awake()
    {
        _eventSystem = GetComponentInChildren<EventSystem>();

        _hud = Instantiate(_interfaceData.HudPrefab, parent: transform);
        _hud.gameObject.SetActive(false);

        _gameOver = Instantiate(_interfaceData.GameOverPrefab, parent: transform);
        _gameOver.gameObject.SetActive(false);

        _mainMenu = Instantiate(_interfaceData.MainMenuPrefab, parent: transform);
        _mainMenu.gameObject.SetActive(false);

        Game.Events.SessionUpdated.Register(OnSessionUpdated);
        Game.Events.BulletAsteroidCollision.Register(OnAsteroidCollision);
        Game.Events.GameStateChanged.Register(OnGameStateChanged);
    }

    public void Start()
    {

    }

    private void OnGameStateChanged((GameState previous, GameState current) arg)
    {
        switch (arg.current)
        {
            case GameState.Loaded:
                OnGameLoaded();
                break;

            case GameState.Started:
                OnGameStarted();
                break;

            case GameState.GameOver:
                OnGameOver();
                break;
        }
    }

    private void OnGameLoaded()
    {
        _hud.Hide();
        _gameOver.Hide();
        _mainMenu.Show();
    }

    private void OnGameStarted()
    {
        _gameOver.Hide();
        _mainMenu.Hide();
        _hud.Show();
    }

    private void OnGameOver()
    {
        _mainMenu.Hide();
        _hud.Hide();
        _gameOver.Show();
    }

    private void OnAsteroidCollision((Asteroid Asteroid, Bullet Bullet) args)
    {
        _hud.UpdateAsteroidsRemaining(Game.Asteroids.ActiveCount);
    }

    private void OnSessionUpdated(PlayerManager.PlayerSession session)
    {        
        _hud.UpdateLives(session.Lives);
        _hud.UpdateScore(session.Score);
    }
}


