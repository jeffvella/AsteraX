using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Responsible for informing the user that their game session is over
/// and handle interactions such as restarting the game.
/// </summary>
[RequireComponent(typeof(Canvas))]
public class GameOverInterface : MonoBehaviour
{
    public Text ScoreElement;
    public Text AsteroidsDestroyedElement;
    public Button TryAgainElement;

    private void Awake()
    {
        TryAgainElement.onClick.AddListener(TryAgainClicked);
    }

    private void TryAgainClicked()
    {
        Game.StartGame();
    }

    public void Show()
    {
        ScoreElement.text = $"{Game.Player.Session.Score}";
        AsteroidsDestroyedElement.text = $"{Game.Player.Session.AsteroidsDestroyed}"; ;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public bool IsVisible => gameObject.activeInHierarchy;
}



