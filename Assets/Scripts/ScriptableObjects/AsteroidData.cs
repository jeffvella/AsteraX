using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.SocialPlatforms;

[Serializable]
public class AsteroidType
{
    public int Points = 100;
    public int Size  = 1;
    public float Children = 3;
    public float CollisionDamage = 1;
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


