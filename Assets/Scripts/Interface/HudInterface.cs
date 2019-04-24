using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Responsible for updating the heads up display (hud), which contains UI elements that
/// inform the player of important information such as score and lives.
/// </summary>
[RequireComponent(typeof(Canvas))]
public class HudInterface : MonoBehaviour, IGameUserInterface
{
    public Text LivesElement;
    public Text ScoreElement;
    public Text AsteroidsRemainingElement;
    public Canvas Canvas;

    public void UpdateLives(int currentLives)
    {
        LivesElement.text = $"{currentLives} lives left";
    }

    public void UpdateScore(int currentScore)
    {
        ScoreElement.text = $"{currentScore} points";
    }

    public void UpdateAsteroidsRemaining(int currentScore)
    {
        AsteroidsRemainingElement.text = $"{currentScore} remaining";
    }

    public void Reset()
    {
        UpdateAsteroidsRemaining(Game.Asteroids.ActiveCount);
        UpdateScore(Game.Player.Session.Score);
    }

    public void Show()
    {
        Reset();
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void TogglePause(bool value)
    {
        Game.Time.Toggle();   
    }

    public bool IsVisible => gameObject.activeInHierarchy;
}



