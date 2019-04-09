using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DrawBoundsGizmo : MonoBehaviour
{
    [Header("Options")]
    public bool ShowAABB = true;
    public bool ShowOBB = true;
    public bool ShowDuringPlay = true;
    public bool ShowInEditor = true;

    [Header("Colors")]
    public Color BoundingBoxColor = Color.black;
    public Color ActualBoxColor = Color.blue;

    void OnDrawGizmos()
    {        
        if (ShowDuringPlay && Application.isPlaying || ShowInEditor && !Application.isPlaying)
        {
            var collider = GetComponent<Collider>();
            if (collider == null) return;

            if (ShowAABB)
            {
                Gizmos.color = BoundingBoxColor;
                Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
            }

            if (ShowOBB)
            {
                var boxCollider = collider as BoxCollider;
                if (boxCollider != null)
                {
                    Gizmos.color = ActualBoxColor;
                    DrawCube(boxCollider.center, boxCollider.size / 2, transform.localToWorldMatrix);
                }
            }
        }
    }

    public static void DrawCube(Vector3 center, Vector3 extents)
    {
        var points = GetTransformedPoints(center, extents);
        Gizmos.DrawLine(points[0], points[1]);
        Gizmos.DrawLine(points[1], points[3]);
        Gizmos.DrawLine(points[2], points[3]);
        Gizmos.DrawLine(points[2], points[0]);
        Gizmos.DrawLine(points[4], points[5]);
        Gizmos.DrawLine(points[5], points[7]);
        Gizmos.DrawLine(points[6], points[7]);
        Gizmos.DrawLine(points[6], points[4]);
        Gizmos.DrawLine(points[0], points[4]);
        Gizmos.DrawLine(points[1], points[5]);
        Gizmos.DrawLine(points[2], points[6]);
        Gizmos.DrawLine(points[3], points[7]);
    }

    public static Vector3[] GetTransformedPoints(Vector3 center, Vector3 extents) => new[]
    {
        center + new Vector3(extents.x, extents.y, extents.z),
        center + new Vector3(extents.x, extents.y, -extents.z),
        center + new Vector3(extents.x, -extents.y, extents.z),
        center + new Vector3(extents.x, -extents.y, -extents.z),
        center + new Vector3(-extents.x, extents.y, extents.z),
        center + new Vector3(-extents.x, extents.y, -extents.z),
        center + new Vector3(-extents.x, -extents.y, extents.z),
        center + new Vector3(-extents.x, -extents.y, -extents.z),
    };

    private static void DrawCube(Vector3 center, Vector3 extents, Matrix4x4 matrix)
    {        
        var points = GetTransformedPoints(center, extents, matrix);
        Gizmos.DrawLine(points[0], points[1]);
        Gizmos.DrawLine(points[1], points[3]);
        Gizmos.DrawLine(points[2], points[3]);
        Gizmos.DrawLine(points[2], points[0]);
        Gizmos.DrawLine(points[4], points[5]);
        Gizmos.DrawLine(points[5], points[7]);
        Gizmos.DrawLine(points[6], points[7]);
        Gizmos.DrawLine(points[6], points[4]);
        Gizmos.DrawLine(points[0], points[4]);
        Gizmos.DrawLine(points[1], points[5]);
        Gizmos.DrawLine(points[2], points[6]);
        Gizmos.DrawLine(points[3], points[7]);
    }

    public static Vector3[] GetTransformedPoints(Vector3 center, Vector3 extents, Matrix4x4 matrix) => new[]
    {
        matrix.MultiplyPoint3x4(center + new Vector3(extents.x, extents.y, extents.z)),
        matrix.MultiplyPoint3x4(center + new Vector3(extents.x, extents.y, -extents.z)),
        matrix.MultiplyPoint3x4(center + new Vector3(extents.x, -extents.y, extents.z)),
        matrix.MultiplyPoint3x4(center + new Vector3(extents.x, -extents.y, -extents.z)),
        matrix.MultiplyPoint3x4(center + new Vector3(-extents.x, extents.y, extents.z)),
        matrix.MultiplyPoint3x4(center + new Vector3(-extents.x, extents.y, -extents.z)),
        matrix.MultiplyPoint3x4(center + new Vector3(-extents.x, -extents.y, extents.z)),
        matrix.MultiplyPoint3x4(center + new Vector3(-extents.x, -extents.y, -extents.z)),
    };





}

