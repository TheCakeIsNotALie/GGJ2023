using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helpers
{
    private static System.Random rng = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    public static List<Vector2> GenerateRandomConvexPolygon(int n)
    {
        // Generate two lists of random X and Y coordinates
        List<float> xPool = new List<float>(n);
        List<float> yPool = new List<float>(n);

        for (int i = 0; i < n; i++)
        {
            xPool.Add(UnityEngine.Random.Range(0.0f, 1.0f));
            yPool.Add(UnityEngine.Random.Range(0.0f, 1.0f));
        }

        // Sort them
        xPool.Sort();
        yPool.Sort();

        // Isolate the extreme points
        float minX = xPool[0];
        float maxX = xPool[n - 1];
        float minY = yPool[0];
        float maxY = yPool[n - 1];

        // Divide the interior points into two chains & Extract the vector components
        List<float> xVec = new List<float>(n);
        List<float> yVec = new List<float>(n);

        float lastTop = minX, lastBot = minX;

        for (int i = 1; i < n - 1; i++)
        {
            float tmpX = xPool[i];

            if (UnityEngine.Random.Range(0.0f, 1.0f) > 0.5f)
            {
                xVec.Add(tmpX - lastTop);
                lastTop = tmpX;
            }
            else
            {
                xVec.Add(lastBot - tmpX);
                lastBot = tmpX;
            }
        }

        xVec.Add(maxX - lastTop);
        xVec.Add(lastBot - maxX);

        float lastLeft = minY, lastRight = minY;

        for (int i = 1; i < n - 1; i++)
        {
            float tmpY = yPool[i];

            if (UnityEngine.Random.Range(0.0f, 1.0f) > 0.5f)
            {
                yVec.Add(tmpY - lastLeft);
                lastLeft = tmpY;
            }
            else
            {
                yVec.Add(lastRight - tmpY);
                lastRight = tmpY;
            }
        }

        yVec.Add(maxY - lastLeft);
        yVec.Add(lastRight - maxY);

        // Randomly pair up the X- and Y-components
        yVec.Shuffle();

        // Combine the paired up components into vectors
        List<Vector2> vec = new List<Vector2>(n);

        for (int i = 0; i < n; i++)
        {
            vec.Add(new Vector2(xVec[i], yVec[i]));
        }

        // Sort the vectors by angle
        vec.Sort((v1, v2) => Mathf.Atan2(v1.y, v1.x).CompareTo(Mathf.Atan2(v2.y, v2.x)));
        //Collections.sort(vec, Comparator.comparingDouble(v->Math.atan2(v.getY(), v.getX())));

        // Lay them end-to-end
        float x = 0, y = 0;
        float minPolygonX = 0;
        float minPolygonY = 0;
        List<Vector2> points = new List<Vector2>(n);

        for (int i = 0; i < n; i++)
        {
            points.Add(new Vector2(x, y));

            x += vec[i].x;
            y += vec[i].y;

            minPolygonX = Mathf.Min(minPolygonX, x);
            minPolygonY = Mathf.Min(minPolygonY, y);
        }

        // Move the polygon to the original min and max coordinates
        float xShift = minX - minPolygonX;
        float yShift = minY - minPolygonY;

        for (int i = 0; i < n; i++)
        {
            Vector2 p = points[i];
            points[i] = new Vector2(p.x + xShift, p.y + yShift);
        }

        return points;
    }
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
