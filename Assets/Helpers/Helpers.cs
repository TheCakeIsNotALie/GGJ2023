using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helpers 
{
    public static Vector3 CopyV3(Vector3 v)
    {
        return new Vector3(v.x, v.y, v.z);
    }
    public static Vector3 CopyV3(Vector3 v, float? x = null, float? y = null)
    {
        float newX = x ?? v.x;
        float newY = y ?? v.y;
        return new Vector3(newX, newY, v.z);
    }
    public static Vector3 CopyV3(Vector3 v, float? z = null)
    {
        float newZ = z ?? v.z;
        return new Vector3(v.x, v.y, newZ);
    }
    public static Vector3 CopyV3(Vector3 v, float? x = null, float? y = null, float? z = null)
    {
        float newX = x ?? v.x;
        float newY = y ?? v.y;
        float newZ = z ?? v.z;
        return new Vector3(newX, newY, newZ);
    }
}
