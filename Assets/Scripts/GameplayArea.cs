using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

[RequireComponent(typeof(BoxCollider))]
public class GameplayArea : MonoBehaviour
{
    private static BoxCollider _collider;
    private static Bounds _paddedBounds;
    public const float Padding = 0.5f;

    void Awake()
    {
        _collider = GetComponent<BoxCollider>();        
        _paddedBounds = _collider.bounds;
        _paddedBounds.Expand(Padding);
    }

    public static Vector3 ClosestPosition(Vector3 position)
    {
        var min = _collider.bounds.min;
        var max = _collider.bounds.max;
        var closestX = Mathf.Max(min.x, Mathf.Min(position.x, max.x));
        var closestY = Mathf.Max(min.y, Mathf.Min(position.y, max.y));
        var closestZ = Mathf.Max(min.z, Mathf.Min(position.z, max.z));
        var closest = new Vector3(closestX, closestY, closestZ);
        return closest;     
    }

    public static Vector3 RandomPointInBounds()
    {
        return new Vector3(
            Random.Range(_collider.bounds.min.x, _collider.bounds.max.x),
            Random.Range(_collider.bounds.min.y, _collider.bounds.max.y),
            Random.Range(_collider.bounds.min.z, _collider.bounds.max.z)
        );
    }

    public static bool TryMoveInsideBounds(GameObject go)
    {
        if (_collider.bounds.Contains(go.transform.position))
            return false;

        var col = go.GetComponent<Collider>();
        if (col != null)
        {      
            var positionInBounds = GetReEntryPosition(col.transform.position, col.bounds);
            go.transform.position = positionInBounds;
            return true;
        }
        return false;
    }

    public static Vector3 GetReEntryPosition(Vector3 position, Bounds bounds)
    {
        var pointOnBounds = _paddedBounds.ClosestPoint(position);
        
        var colliderOverlapPadding = 0.5f;

        var size = (_collider.bounds.extents * 2) + bounds.extents * colliderOverlapPadding;

        var result = new Vector3(pointOnBounds.x, position.y, pointOnBounds.z);

        if (position.z <= _collider.bounds.min.z)
        {
            result.z += size.z;
        }
        if (position.z > _collider.bounds.max.z)
        {
            result.z -= size.z;
        }
        if (position.x <= _collider.bounds.min.x)
        {
            result.x += size.x;
        }
        if (position.x > _collider.bounds.max.x)
        {
            result.x -= size.x;
        }

        //if (position.z <= _collider.bounds.min.z)
        //{
        //    result.z += size.z;
        //}
        //else if (position.z > _collider.bounds.max.z)
        //{
        //    result.z -= size.z ;
        //}
        //if (position.x <= _collider.bounds.min.x)
        //{
        //    result.x += size.x;
        //}
        //else if (position.x > _collider.bounds.max.x)
        //{
        //    result.x -= size.x;
        //}

        Debug.DrawLine(pointOnBounds, result + Vector3.zero * 0.1f, Color.yellow);
        return result;
    }

    public static Vector3 GetFlippedReEntryPosition(Vector3 position)
    {        
        var pointOnBounds = _collider.ClosestPointOnBounds(position);
        var size = _collider.bounds.extents * 2;
        var result = new Vector3(pointOnBounds.x, position.y, pointOnBounds.z);

        if (position.z <= _collider.bounds.min.z)
        {
            result.z += size.z;      
            result.x += size.x - (result.x - _collider.bounds.min.x) * 2;
        }
        else if (position.z > _collider.bounds.max.z)
        {
            result.z -= size.z;  
            result.x += size.x - (result.x - _collider.bounds.min.x) * 2;
        }
        else if(position.x <= _collider.bounds.min.x)
        {
            result.x += size.x;   
            result.z += size.z - (result.z - _collider.bounds.min.z) * 2;
        }
        else if (position.x > _collider.bounds.max.x)
        {
            result.x -= size.x;
            result.z += size.z - (result.z - _collider.bounds.min.z) * 2;
        }

        //Debug.DrawLine(pointOnBounds, result, Color.red);
        return result;
    }

    public static Bounds GetBounds()
    {
        return _collider.bounds;
    }
}


