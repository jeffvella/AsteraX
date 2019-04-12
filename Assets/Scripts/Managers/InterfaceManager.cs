using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Events;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.VFX;
using Object = UnityEngine.Object;

/// <summary>
/// The interface manager monitors game events and controls how to represent that information in the interface.
/// </summary>
public class InterfaceManager : MonoBehaviour
{
    [SerializeField]
    private InterfaceData _interfaceData;
    private HudInterface _hud;
    private GameOverInterface _gameOver;
    private EventSystem _eventSystem;

    public void Awake()
    {
        _eventSystem = GetComponentInChildren<EventSystem>();

        _hud = Instantiate(_interfaceData.HudPrefab, parent: transform);
        _hud.gameObject.SetActive(false);

        _gameOver = Instantiate(_interfaceData.GameOverPrefab, parent: transform);
        _gameOver.gameObject.SetActive(false);
    }

    public void Start()
    {
        Game.Events.OnSessionUpdated.Register(OnSessionUpdated);
        Game.Events.OnBulletAsteroidCollision.Register(OnAsteroidCollision);
        Game.Events.OnGameStateChanged.Register(OnGameStateChanged);
    }

    private void OnGameStateChanged((GameState previous, GameState current) arg)
    {
        switch (arg.current)
        {
            case GameState.GameStarted:
                OnGameStarted();
                break;

            case GameState.GameOver:
                OnGameOver();
                break;
        }
    }

    private void OnGameStarted()
    {
        _hud.Show();
        _gameOver.Hide();
    }

    private void OnGameOver()
    {
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


