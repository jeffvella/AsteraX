using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Debug = System.Diagnostics.Debug;

public class PlayerManager : MonoBehaviour
{
    [Header("Setup")]
    public PlayerData PlayerData;

    public GameObject Ship => _ship;
    private GameObject _ship;

    public ShipController ShipController => _shipController;
    private ShipController _shipController;

    private List<Vector3> _spawnPositions;

    public void Awake()
    {

    }

    public ShipController SpawnShip()
    {
        if (_spawnPositions == null)
        {
            _spawnPositions = GenerateSpiralSpawnPositions();
        }

        var spawnPosition = GetSafeSpiralSpawnPosition(); // GetExpandScanSafeSpawnPosition();
        _ship = Instantiate(PlayerData.ShipType.ShipPrefab, spawnPosition, Quaternion.identity);
        _shipController = _ship.GetComponent<ShipController>();
        _shipController.ShipType = PlayerData.ShipType;
        _shipController.SetState(ShipState.Alive);

        return _shipController ?? throw new InvalidOperationException("Player ship prefab must have a ship controller attached");
    }

    /// <summary>
    /// Generate a spiral of points so that they can be checked from the center outwards.
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

    private Vector3 GetDynamicScanSafePosition()
    {
        var startPosition = Game.Wrap.Bounds.center;
        var unsafeAreas = Game.Asteroids.Select(a => a.Bounds).ToArray();

        var searchPositions = new Queue<Vector3>();
        searchPositions.Enqueue(startPosition);

        int startingSearchRadius = 6;
        int maxSearchIterationsPerRadius = 10;

        // Search increasingly smaller spaces if a safe spot can't be found.
        for (int r = startingSearchRadius - 1; r >= 1; r--)
        {
            var searchSize = new Vector3(r, r, r);
            for (int i = 0; i < maxSearchIterationsPerRadius; i++)
            {
                // Check the current position (next in queue) against all unsafeAreas
                if (TryExpandAndFindSafePosition(searchPositions, searchSize, unsafeAreas, out Vector3 safeSpot))
                {
                    //EditorApplication.isPaused = true;
                    return safeSpot;
                }
            }
        }

        //EditorApplication.isPaused = true;
        return startPosition;
    }

    private static bool TryExpandAndFindSafePosition(Queue<Vector3> searchPositions, Vector3 searchSize, Bounds[] unsafeAreas, out Vector3 safeSpot)
    {
        var searchPosition = searchPositions.Dequeue();
        var searchArea = new Bounds(searchPosition, searchSize);

        //DebugExtension.DebugCircle(searchArea.center, Vector3.up, Color.blue, Math.Max(searchArea.extents.x, searchArea.extents.z));
        //DebugExtension.DebugPoint(searchPosition, Color.white);

        for (var i = 0; i < unsafeAreas.Length; i++)
        {
            var unsafeArea = unsafeAreas[i];

            // Add some randomness to point selection for when the asteroids are clustered together
            var referencePoint = i % 2 == 0 ? searchPosition : Game.Wrap.RandomPointInBounds(1f);

            // Always queue up at least one new search position before leaving method.
            var exploreArea = unsafeAreas[UnityEngine.Random.Range(i, unsafeAreas.Length)];
            var pointOnAreaBounds = exploreArea.ClosestPoint(referencePoint);
            var halfwayPoint = Vector3.Slerp(pointOnAreaBounds, referencePoint, 0.5f);
            searchPositions.Enqueue(halfwayPoint);
       
            //DebugExtension.DebugPoint(halfwayPoint, Color.yellow);
            //DebugExtension.DebugCircle(unsafeArea.center, Vector3.up, Color.yellow, Math.Max(unsafeArea.extents.x, unsafeArea.extents.z));

            if (unsafeArea.Intersects(searchArea))
            {               
                safeSpot = default;
                return false;
            }
        }

        if (!Game.Wrap.Contains(searchArea, 2f))
        {
            safeSpot = default;
            return false;
        }

        safeSpot = searchPosition;
        return true;
    }

    private List<Vector3> GenerateGridSpawnPositions(float size = 5f, float edgePadding = 3f)
    {
        var bounds = Game.Wrap.Bounds;
        var result = new List<Vector3>();

        for (float x = bounds.min.x + edgePadding; x < bounds.max.x - edgePadding; x += size)
        {
            for (float z = bounds.min.z + edgePadding; z < bounds.max.z - edgePadding; z += size)
            {
                var pos = new Vector3(x, bounds.center.y, z);
                result.Add(pos);
                //DebugExtension.DebugPoint(pos);
            }
        }
        return result;
    }


}


