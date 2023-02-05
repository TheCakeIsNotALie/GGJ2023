using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class MathHelper 
{
    public static float RandomStdDev(float mean = 0, float stdDev = 1)
    {
        return mean + stdDev * Mathf.Sqrt(-2 * Mathf.Log(UnityEngine.Random.value)) * Mathf.Sin(2 * Mathf.PI * UnityEngine.Random.value);
    }

    //linePnt - point the line passes through
    //lineDir - unit vector in direction of line, either direction works
    //pnt - the point to find nearest on line for
    public static Vector3 NearestPointOnLine(Vector3 linePnt, Vector3 lineDir, Vector3 pnt)
    {
        lineDir.Normalize();//this needs to be a unit vector
        var v = pnt - linePnt;
        var d = Vector3.Dot(v, lineDir);
        return linePnt + lineDir * d;
    }

    // For finite lines:
    public static Vector3 NearestPointOnSegment(Vector3 point, Vector3 seg_start, Vector3 seg_end)
    {
        Vector3 line_direction = seg_end - seg_start;
        float line_length = line_direction.magnitude;
        line_direction.Normalize();
        float project_length = Mathf.Clamp(Vector3.Dot(point - seg_start, line_direction), 0f, line_length);
        return seg_start + line_direction * project_length;
    }

}
