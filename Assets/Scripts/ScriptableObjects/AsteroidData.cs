using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[Serializable]
public class AsteroidType
{
    public int Points = 100;
    public int Size  = 1;
    public float CollisionDamage = 1;

    public override string ToString() => $"{nameof(AsteroidType)}: Size={Size}, Points={Points}";
}

[Serializable]
public class LevelDefinition
{
    public string Id;
    public string LevelName;
    public int StartingAsteroids;
    public int AsteroidSplits;
    public List<AsteroidData> Asteroids;    
}

[CreateAssetMenu]
public class AsteroidData : ScriptableObject
{
    public List<GameObject> AsteroidPrefabs;
    public List<AsteroidType> AsteroidTypes;
    public List<ParticleEffectData> AsteroidEffects;
    public float MinMoveSpeed = 5f;
    public float MaxMoveSpeed = 10f;
    public float MinRotationSpeed = 1f;
    public float MaxRotationSpeed = 5f;
    public int StartingPoolSize = 20;
}
