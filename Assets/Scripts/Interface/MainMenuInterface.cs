using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// handles UI for the title screen, responsible for getting the user into the game.
/// </summary>
[RequireComponent(typeof(Canvas))]
public class MainMenuInterface : MonoBehaviour, IGameUserInterface
{
    public Button StartGameElement;

    private void Awake()
    {
        StartGameElement.onClick.AddListener(StartGameClicked);
    }

    private void StartGameClicked()
    {        
        Game.StartGame();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public bool IsVisible => gameObject.activeInHierarchy;
}