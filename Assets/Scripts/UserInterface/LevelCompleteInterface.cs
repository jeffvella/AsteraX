using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Responsible for informing the user that their game session is over
/// and handle interactions such as restarting the game.
/// </summary>
[RequireComponent(typeof(Canvas))]
public class LevelCompleteInterface : MonoBehaviour, IGameUserInterface
{
    public Text CurrentLevelNameElement;
    public Text ScoreElement;
    public Text AsteroidsDestroyedElement;
    public Button NextLevelElement;

    private void Awake()
    {
        NextLevelElement.onClick.AddListener(NextLevelClicked);
    }

    private void NextLevelClicked()
    {
        Game.Levels.NextLevel();
    }

    public void Show()
    {
        ScoreElement.text = $"{Game.Player.Session.Score}";
        AsteroidsDestroyedElement.text = $"{Game.Player.Session.AsteroidsDestroyed}";
        CurrentLevelNameElement.text = Game.Levels.CurrentLevel.LevelName;
        gameObject.SetActive(true);

        Game.ResetGame();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public bool IsVisible => gameObject.activeInHierarchy;
}



