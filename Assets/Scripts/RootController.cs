using System.Collections.Generic;
using TMPro;
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
    [SerializeField]
    private float gravelPricePerMeter = 5f;
    [SerializeField]
    private float gravelCheckGranularity = 0.1f;
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

    public HashSet<WaterPocketBehaviour> GetConnectedWaterPockets() {
        return rootTree.linkedWaterPockets;
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
                var computedPrice = CheckForSegmentPrice(mouseWorldPos);

                textRenderer.text = "-" + computedPrice.ToString("0");
                if (!gameManager.CanBuy(computedPrice))
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
                    GenerateNewRootSegment(
                        snapPoint.before,
                        snapPoint.after,
                        Helpers.CopyV3(previewStartPoint.Value, zPosition),
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

    void GenerateNewRootSegment(RootTree.RootNode before, RootTree.RootNode after, Vector3 start, Vector3 end, bool left)
    {
        print("Generating new segment");

        var price = CheckForSegmentPrice(end);
        if (!gameManager.Buy(price))
        {
            print("Cannot create segment, aborting creation");
            return;
        }

        var insertNode = new RootTree.RootNode(start, before);
        var endNode = new RootTree.RootNode(end, insertNode);
        rootTree.InsertIntersection(insertNode, before, after);
        rootTree.InsertLeaf(endNode, insertNode);

        // clear preview inputs for next inputs
        previewStartPoint = null;
    }

    float CheckForSegmentPrice(Vector2 mouseWorldPosition)
    {
        float normalDistance = 0f;
        float gravelDistance = 0f;
        List<RaycastHit2D> hits = new List<RaycastHit2D>();
        Physics2D.Linecast(previewStartPoint.Value, mouseWorldPosition, new ContactFilter2D(), hits);

        List<Collider2D> gravelColliders = new List<Collider2D>();
        Vector2 currentLocation = Vector2.zero;
        foreach (var hit in hits)
        {
            var gpb = hit.collider.gameObject.GetComponent<GravelPatchBehaviour>();
            if (gpb != null)
            {
                gravelColliders.Add(hit.collider);
                currentLocation = hit.point;
                normalDistance = hit.distance;
            }
        }

        if (gravelColliders.Count > 0)
        {
            Vector2 step = ((Vector2)mouseWorldPosition - currentLocation).normalized * gravelCheckGranularity;
            int stepsNeeded = (int)(Vector2.Distance(currentLocation, mouseWorldPosition) / gravelCheckGranularity);
            for (int i = 0; i < stepsNeeded; i++)
            {
                currentLocation = currentLocation + step;
                bool inGravel = false;
                foreach (var collider in gravelColliders)
                {
                    Vector2 closest = collider.ClosestPoint(currentLocation);
                    if (closest == currentLocation)
                    {
                        inGravel = true;
                        break;
                    }
                }

                if (inGravel)
                {
                    gravelDistance += gravelCheckGranularity;
                }
                else
                {
                    normalDistance += gravelCheckGranularity;
                }
            }
        }
        else
        {
            normalDistance = Vector3.Distance(previewStartPoint.Value, mouseWorldPosition);
        }
        //print("normal : " + normalDistance + ", gravel : " + gravelDistance);
        return normalDistance * pricePerMeter + gravelDistance * gravelPricePerMeter;
    }
}
