using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.SocialPlatforms;

[Serializable]
public class AsteroidType
{
    public int Points;
    public int Size;
    public float Children;
}

[CreateAssetMenu]
public class AsteroidData : ScriptableObject
{
    public List<GameObject> AsteroidPrefabs;
    public List<AsteroidType> AsteroidTypes;
    public float MinMoveSpeed = 5f;
    public float MaxMoveSpeed = 10f;
    public float MinRotationSpeed = 1f;
    public float MaxRotationSpeed = 5f;
    public int StartingPoolSize = 20;
}


