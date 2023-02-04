using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using static UnityEditor.Progress;

public class RootBehaviour : MonoBehaviour
{
    LineRenderer lineRenderer;
    PolygonCollider2D collider;
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        collider = GetComponent<PolygonCollider2D>();
    }

    void Update()
    {
        Mesh mesh = new Mesh();
        lineRenderer.BakeMesh(mesh, true);
        collider.SetPath(0, mesh.vertices.Select(x => (Vector2)x).ToList());
    }

    private void OnC(Collision2D collision)
    {
        print("Collision enter");
    }
}
