using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class AchievementData : ScriptableObject
{
    public List<AchievementDefinition> Acheivements;
}

[Serializable]
public struct AchievementDefinition
{
    /// <summary>
    ///  Language independent identifier
    /// </summary>
    [Tooltip("Title of the achievement shown to the user")]
    public string InternalId;

    /// <summary>
    /// Title of the achievement shown to the user
    /// </summary>
    [Tooltip("Title of the achievement shown to the user")]
    public string DisplayName;

    /// <summary>
    /// The second line of text in the achievement shown to the user.
    /// </summary>
    [Tooltip("The second line of text in the achievement shown to the user. Use # to represent the requirement amount specified")]
    public string Description;

    /// <summary>
    /// What is required to receive the achievement
    /// </summary>
    [Tooltip("What is required to receive the achievement")]
    public AchievementRequirement RequirementType;

    /// <summary>
    /// The amount of corresponding requirement, eg. 1000 for bullets fired.
    /// </summary>
    [Tooltip("The amount of corresponding requirement, eg. 1000 for bullets fired.")]
    public int RequirementValue;

    public override string ToString() 
        => $"{nameof(AchievementDefinition)}: {InternalId} {RequirementType} {RequirementValue}";   
}

public enum AchievementRequirement
{
    None = 0,

    /// <summary>
    /// Number of bullet/asteroid collisions after
    /// </summary>
    BulletHits,

    /// <summary>
    /// Number of bullet/asteroid collisions after
    /// the bullet has wrapped on the screen edge.
    /// </summary>
    WrappedBulletHits,

    /// <summary>
    /// Number of bullets fired (since start of the game).
    /// </summary>
    BulletsFired,

    /// <summary>
    /// Player has a score of certain size or more
    /// </summary>
    ScoreReached,

    /// <summary>
    /// Player has started the specified level
    /// </summary>
    LevelReached,
}








