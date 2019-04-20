using System;
using System.Collections.Generic;
using System.Linq;
using Events;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public struct PlayerSession
    {
        public int Score;
        public int Lives;
        public int AsteroidsDestroyed;
    }

    [Header("Setup")]
    public PlayerData PlayerData;

    public GameObject ShipGo => _shipGo;
    private GameObject _shipGo;

    public Ship Ship => _ship;
    private Ship _ship;

    private List<Vector3> _spawnPositions;

    private PlayerSession _session;
    public PlayerSession Session => _session;

    private void Awake()
    {
        Game.Events.ShipDestroyed.Register(OnShipDestroyed);
        Game.Events.BulletAsteroidCollision.Register(OnBulletAsteroidCollision);
        Game.Events.GameStateChanged.Register(OnGameStateChanged);
        InitializePlayerSession();
    }

    private void OnGameStateChanged((GameState Previous, GameState Current) obj)
    {
        switch (obj.Current)
        {
            case GameState.Started:
                InitializePlayerSession();
                break;
        }
    }

    private void OnBulletAsteroidCollision((Asteroid Asteroid, Bullet Bullet) obj)
    {
        _session.Score += obj.Asteroid.Type.Points;
        _session.AsteroidsDestroyed++;

        Game.Events.SessionUpdated.Raise(_session);
    }

    private void InitializePlayerSession()
    {
        _session = new PlayerSession
        {
            Lives = PlayerData.StartingLives,
            Score = PlayerData.StartingScore,
            AsteroidsDestroyed = 0,
        };

        PlayerData.SessionUpdatedEvent.Raise(_session);
    }

    public Ship SpawnShip()
    {
        if (_spawnPositions == null)
        {
            _spawnPositions = GenerateSpiralSpawnPositions();
        }

        var spawnPosition = GetSafeSpiralSpawnPosition();
        _shipGo = Instantiate(PlayerData.ShipData.ShipPrefab, spawnPosition, Quaternion.identity);
        _ship = _shipGo.GetComponent<Ship>();
        _ship.Initialize(PlayerData.ShipData);
        _ship.SetState(ShipState.Spawning);

        return _ship;
    }

    private void OnShipDestroyed(ShipStateChangedEventInfo info, ShipStatusArgs status)
    {
        _session.Lives--;

        if (_session.Lives > 0)
        {
            SpawnShip();
        }
        
        PlayerData.SessionUpdatedEvent.Raise(_session);
    }

    /// <summary>
    /// Generates a spiral of points (so that they can be checked easily from the center outwards).
    /// </summary>
    private List<Vector3> GenerateSpiralSpawnPositions(float edgePadding = 2f)
    {
        var bounds = Game.Wrap.Bounds;
        var padding = new Vector3(edgePadding, edgePadding, edgePadding);
        var shrunkenBounds = new Bounds(bounds.center, bounds.size - padding);
        var scale = 1.5f;
        var randomOffsetAmount = 1;
        var pointCount = (bounds.size.x + bounds.size.z) * 2;

        var result = new List<Vector3>();
        for (var p = 0; p < pointCount; p++)
        {
            var r = UnityEngine.Random.Range(-1f, 1f);
            var pSqrt = (float)Math.Sqrt(p);

            float pX = scale * pSqrt * (float)Math.Cos(p + Math.PI) + (r * randomOffsetAmount);
            float pY = scale * pSqrt * (float)Math.Sin(p + Math.PI) + (r * randomOffsetAmount);

            var pos = new Vector3(pX, shrunkenBounds.center.y, pY);
            if (shrunkenBounds.Contains(pos))
            {
                result.Add(pos);
                //DebugExtension.DebugPoint(pos, Color.magenta);
            }
        }
        return result;
    }

    /// <summary>
    /// Finds a position as close to the center as possible without any asteroids nearby.
    /// </summary>
    private Vector3 GetSafeSpiralSpawnPosition(float radius = 6f)
    {
        var startPosition = Game.Wrap.Bounds.center;
        var unsafeAreas = Game.Asteroids.Select(a => a.Bounds).ToArray();

        if (TryFindSafePosition(unsafeAreas, _spawnPositions.ToArray(), startPosition, radius, out var safePosition))
        {
            return safePosition;
        }
        if (TryFindSafePosition(unsafeAreas, _spawnPositions.ToArray(), startPosition, radius/2, out safePosition))
        {
            return safePosition;
        }
        return startPosition;
    }

    private bool TryFindSafePosition(Bounds[] unsafeAreas, Vector3[] positions, Vector3 startPosition, float radius, out Vector3 result)
    {
        var searchSize = new Vector3(radius, radius, radius);
        var spawnArea = new Bounds(startPosition, searchSize);

        for (int i = 0; i < positions.Length; i++)
        {
            bool isSafe = true;
            for (int j = 0; j < unsafeAreas.Length; j++)
            {
                Vector3 pos = positions[i];
                Bounds unsafeArea = unsafeAreas[j];
                spawnArea.center = pos;

                //DebugExtension.DebugPoint(pos, Color.yellow);
                //DebugExtension.DebugCircle(unsafeArea.center, Vector3.up, Color.yellow, Math.Max(unsafeArea.extents.x, unsafeArea.extents.z));

                if (unsafeArea.Intersects(spawnArea))
                {
                    //DebugExtension.DebugPoint(pos, Color.red);
                    //DebugExtension.DebugCircle(unsafeArea.center, Vector3.up, Color.red, Math.Max(unsafeArea.extents.x, unsafeArea.extents.z));

                    isSafe = false;
                    break;
                }
            }
            if (isSafe)
            {
                {
                    result = positions[i];
                    return true;
                }
            }
        }
        result = default;
        return false;
    }
}


