using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class RootController : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private RootTree rootTree;
    [SerializeField]
    private GameObject snapNodePrefab;
    [SerializeField]
    private GameObject previewPlacementPrefab; 
    [SerializeField]
    private Color previewErrorColor = Color.red;

    [Header("Placement")]
    [SerializeField]
    private float pricePerMeter = 2.5f;
    public bool hasFocus = false;
    [SerializeField]
    private float snapRange = 1f;
    [SerializeField]
    private float zPosition = -1;

    private GameObject previewSnap;
    private GameObject previewPlacement;
    [SerializeField]
    private Color previewInitialColor = Color.white;
    [SerializeField]
    private float zPreviewPosition = -3;
    private Color previewTextInitialColor = Color.white;
    private RootTree.PointSearch snapPoint;

    private Vector3? previewStartPoint = null;
    // Start is called before the first frame update
    void Start()
    {
        previewPlacement = Instantiate(previewPlacementPrefab);
        previewPlacement.SetActive(false);
        LineRenderer lineRenderer = previewPlacement.GetComponent<LineRenderer>();
        TextMeshPro textRenderer = previewPlacement.GetComponentInChildren<TextMeshPro>();

        previewSnap = Instantiate(snapNodePrefab);
        previewSnap.SetActive(false);
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
                LineRenderer lineRenderer = previewPlacement.GetComponent<LineRenderer>();
                TextMeshPro textRenderer = previewPlacement.GetComponentInChildren<TextMeshPro>();

                var points = new Vector3[2];
                points[0] = previewStartPoint.Value;
                points[1] = Helpers.CopyV3(mouseWorldPos, zPreviewPosition);

                // check if the segment can be bought
                var distance = Vector2.Distance(points[0], points[1]);
                var price = distance * pricePerMeter;
                textRenderer.text = "-" + price.ToString("0");
                if (!gameManager.CanBuy(price))
                {
                    lineRenderer.startColor = previewErrorColor;
                    lineRenderer.endColor = previewErrorColor;
                    textRenderer.color = previewErrorColor;
                }
                else
                {
                    lineRenderer.startColor = previewInitialColor;
                    lineRenderer.endColor = previewInitialColor;
                    textRenderer.color = previewTextInitialColor;
                }

                lineRenderer.positionCount = 2;
                lineRenderer.SetPositions(points);
                previewPlacement.transform.position = previewStartPoint.Value;
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
                    previewSnap.transform.position = Helpers.CopyV3(snapPoint.pointOnSegment, zPreviewPosition);
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
                else if (previewStartPoint != null)
                {
                    generateNewRootSegment(
                        snapPoint.parent,
                        Helpers.CopyV3(previewStartPoint.Value,zPosition),
                        Helpers.CopyV3(mouseWorldPos, zPosition),
                        snapPoint.left
                        );
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

        var distance = Vector2.Distance(start, end);
        var price = distance * pricePerMeter;
        if (!gameManager.Buy(price))
        {
            print("Cannot create segment, aborting creation");
            return;
        }

        var insertNode = new RootTree.RootNode(start, parent);
        var endNode = new RootTree.RootNode(end, insertNode);
        rootTree.InsertIntersection(insertNode, parent, left);
        rootTree.InsertLeaf(endNode, insertNode);

        // clear preview inputs for next inputs
        previewStartPoint = null;
    }
}
