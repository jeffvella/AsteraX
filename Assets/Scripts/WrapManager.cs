using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class WrapManager : MonoBehaviour, IExposedBehavior
{
    //public Bounds Bounds;

    private static Bounds _paddedBounds;
    public const float Padding = 0.5f;

    public Bounds Bounds => _collider.bounds; 

    public BoxCollider Collider => _collider;
    private static BoxCollider _collider;

    private void Awake()
    {
        _collider = GetComponent<BoxCollider>();
        _paddedBounds = Collider.bounds;
        _paddedBounds.Expand(Padding);
    }

    public Vector3 ClosestPosition(Vector3 position)
    {
        var min = _collider.bounds.min;
        var max = _collider.bounds.max;
        var closestX = Mathf.Max(min.x, Mathf.Min(position.x, max.x));
        var closestY = Mathf.Max(min.y, Mathf.Min(position.y, max.y));
        var closestZ = Mathf.Max(min.z, Mathf.Min(position.z, max.z));
        var closest = new Vector3(closestX, closestY, closestZ);
        return closest;     
    }

    public Vector3 RandomPointInBounds(float xyInset = 0)
    {
        return new Vector3(
            Random.Range(_collider.bounds.min.x + xyInset, _collider.bounds.max.x - xyInset),
            _collider.center.y, //Random.Range(_collider.bounds.min.y, _collider.bounds.max.y),
            Random.Range(_collider.bounds.min.z + xyInset, _collider.bounds.max.z - xyInset)
        );
    }


    public bool TryMoveInsideBounds(GameObject go)
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

    public Vector3 GetReEntryPosition(Vector3 position, Bounds bounds)
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

        Debug.DrawLine(pointOnBounds, result + Vector3.zero * 0.1f, Color.yellow);
        return result;
    }

    public Vector3 GetFlippedReEntryPosition(Vector3 position)
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

    public bool Contains(Bounds b, float inset = 0)
    {
        var a = _collider.bounds;
        var result = b.min.x >= a.min.x + inset && b.max.x <= a.max.x - inset &&
                     b.min.y >= a.min.y + inset && b.max.y <= a.max.y - inset &&
                     b.min.z >= a.min.z + inset && b.max.z <= a.max.z - inset;

        //DebugExtension.DebugBounds(b, result ? Color.blue : Color.cyan);
        return result;
    }

    void IExposedBehavior.Awake() => Awake();
    void IExposedBehavior.Update() { }
}


