using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.UI;

/// <summary>
/// Responsible for the intro to a new level, which lets the user
/// know the level name and difficulty details
/// </summary>
[RequireComponent(typeof(Canvas))]
public class LevelStartedInterface : MonoBehaviour, IGameUserInterface
{
    public Text CurrentLevelNameElement;
    public Text AsteroidCountElement;
    public Text AsteroidSplitsElement;

    public PlayableDirector Director;

    private void Awake()
    {

    }

    public void Show()
    {
        AsteroidCountElement.text = $"{Game.Levels.CurrentLevel.StartingAsteroids}";
        AsteroidSplitsElement.text = $"{Game.Levels.CurrentLevel.AsteroidSplits}";
        CurrentLevelNameElement.text = Game.Levels.CurrentLevel.LevelName;
        gameObject.SetActive(true);
        Director.Restart();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void IsPaused(bool value)
    {
        if (value)
        {
            Game.Time.Pause();
        }
        else
        {
            Game.Time.Resume();
        }
    }

    public bool IsVisible => gameObject.activeInHierarchy;
}

public static class PlayableDirectorExtensions
{
    public static void Restart(this PlayableDirector director)
    {
        director.time = director.initialTime;
        director.Play();
    }
}


