using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class GravelPatchBehaviour : MonoBehaviour
{
    public Vector2[] path;
    public int segments;
    public float width;

    // Start is called before the first frame update
    void Start()
    {
        float angle = 0;

        var ssc = GetComponent<SpriteShapeController>();
        ssc.spline.Clear();
        Vector2 Bprime = Vector2.zero;
        for (int i = 1; i < path.Length; i++)
        {
            var a = path[i-1];
            a.Normalize();
            var b = path[i];
            b.Normalize();
            var ab = b - a;
            var p = new Vector2(
                -ab.y,
               ab.x
                );
            var Aprime = path[i - 1] + p * (width * 1 + UnityEngine.Random.Range(-0.1f, 0.1f));
            Bprime = path[i] + p * (width * 1 + UnityEngine.Random.Range(-0.1f, 0.1f));
            ssc.spline.InsertPointAt(i - 1, Aprime);
        }
        ssc.spline.InsertPointAt(path.Length-1, Bprime);

        // go backwards and subtract width to do the other side
        for (int i = path.Length - 1; i >= 0; i--)
        {
            var a = path[i + 1];
            a.Normalize();
            var b = path[i];
            b.Normalize();
            var ab = b - a;
            var p = new Vector2(
                -ab.y,
               ab.x
                );
            var Aprime = path[i - 1] - p * (width * 1 + UnityEngine.Random.Range(-0.1f, 0.1f));
            Bprime = path[i] - p * (width * 1 + UnityEngine.Random.Range(-0.1f, 0.1f));
            ssc.spline.InsertPointAt(i - 1, Aprime);
        }
        ssc.spline.InsertPointAt(path.Length - 1, Bprime);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
