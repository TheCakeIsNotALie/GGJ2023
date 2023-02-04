using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class RootController : MonoBehaviour
{
    public GameObject snapNodePrefab;
    public GameObject previewPlacementPrefab;

    private RootTree rootTree;
    private GameObject previewSnap;
    private GameObject previewPlacement;
    private RootTree.PointSearch snapPoint;

    public bool hasFocus = false;

    public float snapRange = 1f;
    public float zPosition = -1;

    private Vector3? previewStartPoint = null;
    // Start is called before the first frame update
    void Start()
    {
        previewPlacement = Instantiate(previewPlacementPrefab);
        previewPlacement.SetActive(false);

        previewSnap = Instantiate(snapNodePrefab);
        previewSnap.SetActive(false);

        rootTree = GetComponent<RootTree>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hasFocus)
        {
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = zPosition;

            if (previewStartPoint != null)
            {
                LineRenderer rootRenderer = previewPlacement.GetComponent<LineRenderer>();

                var points = new Vector3[2];
                points[0] = previewStartPoint.Value;
                points[1] = mouseWorldPos;

                rootRenderer.positionCount = 2;
                rootRenderer.SetPositions(points);
                previewPlacement.SetActive(true);
            }
            else
            {
                // only show snap point if the start point hasn't been selected yet
                var snapCandidate = rootTree.ClosestPointOnTree(mouseWorldPos);
                // compare 2d only to avoid z offset errors
                if (snapCandidate.distance <= snapRange)
                {
                    snapPoint = snapCandidate;
                    previewSnap.transform.position = snapPoint.pointOnSegment;
                    previewSnap.SetActive(true);
                }
                else
                {
                    previewSnap.SetActive(false);
                }

                previewPlacement.SetActive(false);
            }


            if (Input.GetButtonDown("Click1"))
            {
                //print("start point : " + previewStartPoint == null + ", snap active" + previewSnap.activeSelf);
                if (previewStartPoint == null && previewSnap.activeSelf)
                {
                    previewStartPoint = previewSnap.transform.position;
                }
                else if(previewStartPoint != null)
                {
                    generateNewRootSegment(snapPoint.parent, previewStartPoint.Value, mouseWorldPos, snapPoint.left);
                }
            }
            //clear preview on right click
            if (Input.GetButtonDown("Click2"))
            {
                snapPoint = null;
                previewStartPoint = null;
            }
        }
    }

    void generateNewRootSegment(RootTree.RootNode parent, Vector3 start, Vector3 end, bool left)
    {
        print("Generating new segment");
        var insertNode = new RootTree.RootNode(start, parent);
        var endNode = new RootTree.RootNode(end, insertNode);
        rootTree.InsertIntersection(insertNode, parent, left);
        rootTree.InsertLeaf(endNode, insertNode);

        // clear preview inputs for next inputs
        previewStartPoint = null;
    }
}
