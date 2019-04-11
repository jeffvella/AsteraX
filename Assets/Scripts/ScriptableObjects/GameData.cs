using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.SocialPlatforms;

[Serializable]
public class ManagerDefinitions
{
    public BulletManager BulletManagerPrefab;
    public PlayerManager PlayerManagerPrefab;
    public AsteroidManager AsteroidManagerPrefab;
    public WrapManager WrapManager;
}

[CreateAssetMenu]
public class GameData : ScriptableObject
{
    public ManagerDefinitions Managers;
    public int StartingAsteroids = 10;
}



