using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public AsteroidManager AsteroidManager;

    public int StartingAsteroids = 20;
    private GameController _instance;

    private GameController()
    {
        _instance = this;
    }

    void Awake()
    {

    }

    void Start()
    {        
        SpawnAsteroids();
    }

    private void SpawnAsteroids()
    {
        for (int i = 0; i < StartingAsteroids; i++)
        {
            AsteroidManager.SpawnAsteroid();
        }
    }

    void Update()
    {
        
    }


}
