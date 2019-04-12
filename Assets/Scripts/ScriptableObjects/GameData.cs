using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using Events;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using Debug = UnityEngine.Debug;

[Serializable]
public class ManagerDefinitions
{
    public BulletManager BulletManagerPrefab;
    public PlayerManager PlayerManagerPrefab;
    public AsteroidManager AsteroidManagerPrefab;
    public WrapManager WrapManagerPrefab;
    public InterfaceManager InterfaceManagerPrefab;
}

[Serializable]
public class EventReferences
{
    public PlayerSessionUpdatedEvent OnSessionUpdated;
    public ShipStateChangedEvent OnShipDestroyed;
    public AsteroidBulletCollisionEvent OnBulletAsteroidCollision;
    public GameStateChangedEvent OnGameStateChanged;
}

[CreateAssetMenu]
public class GameData : ScriptableObject
{
    public ManagerDefinitions Managers;
    public EventReferences Events;
    public int StartingAsteroids = 10;
}



