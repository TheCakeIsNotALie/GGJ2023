using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class GravelPatchBehaviour : MonoBehaviour
{
    public List<Vector3> path;

    // Start is called before the first frame update
    void Start()
    {
        var ssc = GetComponent<SpriteShapeController>();
        ssc.spline.Clear();
        for (int i = 0; i < path.Count; i++)
        {
            ssc.spline.InsertPointAt(i, path[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
