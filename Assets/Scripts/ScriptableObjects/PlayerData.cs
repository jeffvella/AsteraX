using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.SocialPlatforms;

[Serializable]
public class ShipType
{
    public GameObject ShipPrefab;
    public float MaxSpeed = 10f;
    public float TiltDegrees = 15f;
    public float StartingHealth = 1;
    public int Lives = 3;
}

[CreateAssetMenu]
public class PlayerData : ScriptableObject
{
    public ShipType ShipType;
}



