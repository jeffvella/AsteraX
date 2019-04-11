//using System;
//using System.Collections;
//using System.Collections.Generic;
//using NUnit.Framework;
//using UnityEditor;
//using UnityEditor.SceneManagement;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//using UnityEngine.TestTools;

//public class PlayerManagerTests
//{
//    //[SetUp]
//    //public void SetupScene()
//    //{
//    //    SceneManager.CreateScene("_Scene_01");
//    //    //EditorSceneManager.NewScene(NewSceneSetup.EmptyScene);
//    //    var test = GameObject.FindObjectOfType<WrapManager>();
//    //}

//    //[TearDown]
//    //public void UnloadScene()
//    //{
//    //    SceneManager.UnloadSceneAsync("_Scene_01");
//    //}

//    [Test]
//    public void PlayerManager_ShipSpawns()
//    {
//        var go = new GameObject();
//        var playerManager = go.AddComponent<PlayerManager>();
//        ShipController ship = playerManager.SpawnShip();
//        Assert.IsFalse(ship.isActiveAndEnabled);
//    }

//    [Test]
//    public void PlayerManager_ShipSpawnsAliveWithHealth()
//    {
//        var go = new GameObject();
//        var playerManager = go.AddComponent<PlayerManager>();
//        ShipController ship = playerManager.SpawnShip();
//        Assert.IsFalse(ship.Status.Health > 0);
//    }

//    [Test]
//    public void PlayerManager_ShipSpawnsInSafeZone()
//    {
//        var go = new GameObject();
//        var playerManager = go.AddComponent<PlayerManager>();
//        var asteroidManager = go.AddComponent<AsteroidManager>();

//        ShipController ship = playerManager.SpawnShip();

//        int asteroidCount = 20;
//        float safeDistance = 1f;

//        List<Asteroid> asteroids = new List<Asteroid>();
//        for (int i = 0; i < asteroidCount; i++)
//        {
//            asteroidManager.SpawnAsteroid();
//        }

//        foreach (Asteroid asteroid in asteroidManager)
//        {
//            float distance = Vector3.Distance(ship.transform.position, asteroid.transform.position);
//            Assert.IsTrue(distance >= safeDistance);
//        }
//    }

//    [Test]
//    public void PlayerManager_ShipDespawnsOnZeroHealth()
//    {
//        var go = new GameObject();
//        var playerManager = go.AddComponent<PlayerManager>();
//        ShipController ship = playerManager.SpawnShip();
//        ship.ApplyDamage(float.MaxValue);

//        Assert.IsTrue(!ship.isActiveAndEnabled);
//        Assert.IsTrue(ship.Status.Health <= 0);
//    }

//}