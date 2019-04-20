using NUnit.Framework;
using UnityEngine;
using Object = UnityEngine.Object;

public class PlayerManagerTests
{
    private WrapManager _gameController;
    private Game _controller;

    [SetUp]
    public void SetupScene()
    {
        var testData = Resources.Load<TestData>("TestData");
        Object.Instantiate(testData.TestScenePrefab);
        Object.Instantiate(testData.GameControllerPrefab);
    }

    [TearDown]
    public void TearDownScene()
    {
        var objects = Object.FindObjectsOfType<GameObject>();
        foreach (var obj in objects)
        {
            Object.DestroyImmediate(obj);
        }
    }

    [Test]
    public void PlayerManager_ShipSpawns()
    {
        Ship ship = Game.Player.SpawnShip();
        Assert.IsTrue(ship.isActiveAndEnabled);
    }

    [Test]
    public void PlayerManager_ShipSpawnsAliveWithHealth()
    {
        Ship ship = Game.Player.SpawnShip();
        Assert.IsTrue(ship.Status.State == ShipState.Alive);
        Assert.IsTrue(ship.Status.Health > 0);
    }

    [Test]
    public void PlayerManager_ShipSpawnsWithinBounds()
    {
        Ship ship = Game.Player.SpawnShip();
        Assert.IsTrue(Game.Wrap.Contains(ship.Bounds));
    }

    [Test]
    public void PlayerManager_ShipSpawnsInSafeZone()
    {
        Game.SpawnAsteroids();

        Ship ship = Game.Player.SpawnShip();

        var safeSize = 6f;
        var safeArea = ship.Bounds;
        safeArea.Expand(safeSize);

        foreach (Asteroid asteroid in Game.Asteroids)
        {
            Assert.IsFalse(asteroid.Bounds.Intersects(safeArea));
        }
    }

    [Test]
    public void PlayerManager_ShipDespawnsOnZeroHealth()
    {
        Ship ship = Game.Player.SpawnShip();
        ship.ApplyDamage(float.MaxValue);

        Assert.IsTrue(!ship.isActiveAndEnabled);
        Assert.IsTrue(ship.Status.Health <= 0);
    }

}