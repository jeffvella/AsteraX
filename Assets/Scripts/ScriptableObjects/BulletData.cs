using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BulletData : ScriptableObject
{
    public GameObject BulletPrefab;
    public float Speed = 10f;
    public float Duration = 2f;
}

