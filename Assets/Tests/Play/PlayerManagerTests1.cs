using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

public class PlayerManagerTests
{
    private WrapManager _gameController;
    private Game _controller;


    [SetUp]
    public void SetupScene()
    {
        //Resources.Load<Scene>("TestData");
        //SceneManager.LoadScene("TestData");
        //EditorSceneManager.OpenScene("TestData");
        var testData = Resources.Load<TestData>("TestData");
        Object.Instantiate(testData.TestScenePrefab);
        Object.Instantiate(testData.GameControllerPrefab);
    }

    [Test]
    public void PlayerManager_ShipSpawns()
    {
        ShipController ship = Game.Player.SpawnShip();
        Assert.IsTrue(ship.isActiveAndEnabled);
    }

    [Test]
    public void PlayerManager_ShipSpawnsAliveWithHealth()
    {
        ShipController ship = Game.Player.SpawnShip();
        Assert.IsTrue(ship.Status.State == ShipState.Alive);
        Assert.IsTrue(ship.Status.Health > 0);
    }

    [Test]
    public void PlayerManager_ShipSpawnsWithinBounds()
    {
        ShipController ship = Game.Player.SpawnShip();
        Assert.IsTrue(Game.Wrap.Contains(ship.Bounds));
    }

    [Test]
    public void PlayerManager_ShipSpawnsInSafeZone()
    {
        ShipController ship = Game.Player.SpawnShip();

        int asteroidCount = 20;
        float safeDistance = 2f;

        List<Asteroid> asteroids = new List<Asteroid>();
        for (int i = 0; i < asteroidCount; i++)
        {
            Game.Asteroids.SpawnAsteroid();
        }

        foreach (Asteroid asteroid in Game.Asteroids)
        {
            float distance = Vector3.Distance(ship.transform.position, asteroid.transform.position);

            if (distance < safeDistance)
            {
                EditorApplication.isPaused = true;
                Assert.IsTrue(distance >= safeDistance);
            }
        }
    }

    [Test]
    public void PlayerManager_ShipDespawnsOnZeroHealth()
    {
        ShipController ship = Game.Player.SpawnShip();
        ship.ApplyDamage(float.MaxValue);

        Assert.IsTrue(!ship.isActiveAndEnabled);
        Assert.IsTrue(ship.Status.Health <= 0);
    }

}