using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BulletData : ScriptableObject
{
    public GameObject BulletPrefab;
    public List<ParticleEffectData> BulletEffects;
    public float Speed = 10f;
    public float Duration = 2f;
    public int StartingPoolSize = 20;
}

