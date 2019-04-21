using System;
using Assets.Scripts.Managers;
using Events;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
public class ManagerDefinitions
{
    public BulletManager BulletManagerPrefab;
    public PlayerManager PlayerManagerPrefab;
    public AsteroidManager AsteroidManagerPrefab;
    public WrapManager WrapManagerPrefab;
    public InterfaceManager InterfaceManagerPrefab;
    public LevelManager LevelManagerPrefab;
    public EffectsManager EffectsManagerPrefab;
}

[Serializable]
public class EventReferences
{
    public PlayerSessionUpdatedEvent SessionUpdated;
    public ShipStateChangedEvent ShipDestroyed;
    public AsteroidBulletCollisionEvent BulletAsteroidCollision;
    public GameStateChangedEvent GameStateChanged;
}

[CreateAssetMenu]
public class GameData : ScriptableObject
{
    public ManagerDefinitions Managers;
    public EventReferences Events;

    public bool GodMode;
    //public int StartingAsteroids = 10;
}



