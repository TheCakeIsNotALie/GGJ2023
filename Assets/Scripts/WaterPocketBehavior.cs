using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class WaterPocketBehavior : MonoBehaviour
{
    public int segments;
    public float width;
    public float height;
    public float waterQuantity;
    public float angleOffset;

    // Start is called before the first frame update
    void Start()
    {
        float angle = 0;

        var ssc = GetComponent<SpriteShapeController>();
        ssc.spline.Clear();
        for (int i = 0; i < segments; i++)
        {
            try
            {
                float rndX = UnityEngine.Random.Range(-0.1f, 0.1f) * width;
                float rndY = UnityEngine.Random.Range(-0.1f, 0.1f) * height;
                ssc.spline.InsertPointAt(i, new Vector3(Mathf.Cos(angle + angleOffset) * width + rndX, Mathf.Sin(angle) * height + rndY));
            }
            catch (ArgumentException ex)
            {
                ssc.spline.InsertPointAt(i, new Vector3(Mathf.Cos(angle + angleOffset) * width, Mathf.Sin(angle) * height));
            }
            angle += 2 * Mathf.PI / segments;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
