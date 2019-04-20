using System;
using Events;
using UnityEngine;

[Serializable]
public class ManagerDefinitions
{
    public BulletManager BulletManagerPrefab;
    public PlayerManager PlayerManagerPrefab;
    public AsteroidManager AsteroidManagerPrefab;
    public WrapManager WrapManagerPrefab;
    public InterfaceManager InterfaceManagerPrefab;
    public EffectsManager EffectsManagerPrefab;
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



