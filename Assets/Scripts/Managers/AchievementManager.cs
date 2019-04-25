using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

/// <summary>
/// Responsible monitoring progress towards achievements, triggering an event when they're attained.
/// </summary>
public class AchievementManager : MonoBehaviour
{
    public AchievementData AchievementData;

    public ILookup<AchievementRequirement, AchievementProgress> Current;

    public class AchievementProgress
    {
        public int Value;
        public bool HasBeenAwarded;
        public AchievementDefinition Definition;

        public bool IsRequirementReached => Value >= Definition.RequirementValue;
    }

    private void Awake()
    {
        Current = AchievementData.Acheivements.ToLookup(k => k.RequirementType, v => new AchievementProgress
        {
            Definition = v
        });

        Game.Events.BulletAsteroidCollision.Register(OnBulletAsteroidCollision);
        Game.Events.BulletFired.Register(OnBulletFired);
        Game.Events.GameStateChanged.Register(OnGameStateChanged);
    }

    private void OnGameStateChanged((GameState Previous, GameState Current) obj)
    {
        bool newLevelReached = obj.Current == GameState.LevelStarted && obj.Previous != GameState.GameOver;
        if (newLevelReached)
        {
            CompareAchievementsOfType(AchievementRequirement.LevelReached, Game.Levels.CurrentLevelNumber);
        }
    }

    private void OnBulletFired(Bullet bullet)
    {
        IncrementAchievementsOfType(AchievementRequirement.BulletsFired);
    }

    private void OnBulletAsteroidCollision((Asteroid Asteroid, Bullet Bullet) obj)
    {
        IncrementAchievementsOfType(AchievementRequirement.BulletHits);

        if (obj.Bullet.HasWrapped)
        {
            IncrementAchievementsOfType(AchievementRequirement.WrappedBulletHits);
        }
    }

    private void CompareAchievementsOfType(AchievementRequirement requirementType, int amount = 1)
    {
        foreach (var progress in Current[requirementType])
        {
            if (!progress.HasBeenAwarded && amount >= progress.Definition.RequirementValue)
            {
                progress.HasBeenAwarded = true;
                TriggerAchievement(progress);
            }
        }
    }

    private void IncrementAchievementsOfType(AchievementRequirement requirementType, int amount = 1)
    {
        foreach (var progress in Current[requirementType])
        {
            progress.Value += amount;

            if (!progress.HasBeenAwarded && progress.IsRequirementReached)
            {
                progress.HasBeenAwarded = true;
                TriggerAchievement(progress);
            }           
        }
    }

    public void TriggerAchievement(AchievementProgress progress)
    {
        Game.Events.AchievementAttained.Raise(progress.Definition);
    }
}




