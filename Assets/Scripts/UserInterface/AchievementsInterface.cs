using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

/// <summary>
/// Responsible for displaying notification UI for achievements
/// </summary>
[RequireComponent(typeof(Canvas))]
public class AchievementsInterface : MonoBehaviour, IGameUserInterface
{
    private readonly Queue<AchievementDefinition> _pendingAchievements = new Queue<AchievementDefinition>();   
    private AchievementDefinition? _currentlyActive;

    public PlayableDirector Director;
    public Text AchievementNameElement;
    public Text AchievementDescriptionElement;

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public bool IsVisible => gameObject.activeInHierarchy;

    public void DisplayAchievement(AchievementDefinition definition)
    {
        _pendingAchievements.Enqueue(definition);
    }

    public void Update()
    {
        if (_currentlyActive == null)
        {
            if (_pendingAchievements.Count > 0)
            {
                Play(_pendingAchievements.Dequeue());
            }
        }
        else if (Director.state != PlayState.Playing)
        {
            _currentlyActive = null;
        }        
    }

    private void Play(AchievementDefinition definition)
    {
        AchievementNameElement.text = definition.DisplayName;
        AchievementDescriptionElement.text = definition.Description.Replace("#", definition.RequirementValue.ToString("N0"));
        Director.Restart();
    }

}




