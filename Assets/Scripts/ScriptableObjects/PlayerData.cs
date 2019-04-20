using System;
using Events;
using UnityEngine;

[Serializable]
public class ShipData
{
    public GameObject ShipPrefab;
    public float MaxSpeed = 10f;
    public float TiltDegrees = 15f;
    public float StartingHealth = 1;
    public ShipStateChangedEvent DestroyedEvent;
    public ParticleEffect SpawnEffect;
    public ParticleEffect DespawnEffect;
    public ParticleEffect ExhaustEffect;
}

[CreateAssetMenu]
public class PlayerData : ScriptableObject
{
    public ShipData ShipData;
    public int StartingLives = 3;
    public int StartingScore = 0;
    public PlayerSessionUpdatedEvent SessionUpdatedEvent;
}




