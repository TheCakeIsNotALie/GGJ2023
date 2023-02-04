using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootController : MonoBehaviour
{
    public GameObject rootsContainer;
    public GameObject rootPrefab;
    public GameObject snapNodePrefab;

    private GameObject previewSnap;
    private GameObject previewRoot;
    private GameObject snapParentRoot;
    private List<GameObject> rootSegments = new List<GameObject>();

    public bool hasFocus = false;

    public float snapRange = 0.75f;
    public float zPosition = -1;
    public float maxRootWidth = 1;

    public float rootWidthGrowthPerMeter = 0.125f;

    private Vector3? previewStartPoint = null;
    private Vector3? previewEndPoint = null;
    // Start is called before the first frame update
    void Start()
    {
        previewRoot = Instantiate(rootPrefab, rootsContainer.transform);
        previewRoot.SetActive(false);

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
                previewEndPoint = mouseWorldPos;
                LineRenderer rootRenderer = previewRoot.GetComponent<LineRenderer>();

                var points = new Vector3[2];
                points[0] = previewStartPoint.Value;
                points[1] = previewEndPoint.Value;

                var distance = Vector3.Distance(points[0], points[1]);
                var widthStart = Mathf.Min(distance * rootWidthGrowthPerMeter, maxRootWidth);
                var widthEnd = Mathf.Min(0, maxRootWidth);

                rootRenderer.positionCount = 2;
                rootRenderer.startWidth = widthStart;
                rootRenderer.endWidth = widthEnd;
                rootRenderer.SetPositions(points);
                previewRoot.SetActive(true);
            }

            bool snapActivated = false;
            Vector3 comparePoint = rootsContainer.transform.position;
            // compare 2d only to avoid z offset errors
            if (Vector2.Distance(comparePoint, mouseWorldPos) <= snapRange)
            {
                snapActivated = true;
                previewSnap.transform.position = comparePoint;
                previewSnap.SetActive(true);
            }
            else
            {
                float nearestDistance = snapRange;
                Vector3? nearestPoint = null;
                foreach (var item in rootSegments)
                {
                    var lr = item.GetComponent<LineRenderer>();
                    int positions = lr.positionCount;
                    var p1 = lr.GetPosition(0);
                    var p2 = lr.GetPosition(positions - 1);

                    var nearest = MathHelper.NearestPointOnSegment(mouseWorldPos, p1, p2);
                    var distance = Vector2.Distance(nearest, mouseWorldPos);
                    if (distance < nearestDistance)
                    {
                        snapParentRoot = item;
                        nearestPoint = nearest;
                        nearestDistance = distance;
                    }
                }

                if (nearestPoint != null)
                {
                    snapActivated = true;
                    previewSnap.transform.position = nearestPoint.Value;
                    previewSnap.SetActive(true);
                }
            }

            if (!snapActivated)
            {
                previewSnap.SetActive(false);
            }


            if (Input.GetButtonDown("Click1") && previewSnap.activeSelf)
            {
                print("start point : " + previewStartPoint + ", snap active" + previewSnap.activeSelf);
                if (previewStartPoint == null)
                {
                    previewStartPoint = previewSnap.transform.position;
                }
                else
                {
                    generateNewRootSegment();
                }
            }
        }
    }

    void generateNewRootSegment()
    {
        var parent = snapParentRoot != null ? snapParentRoot.transform : rootsContainer.transform;
        GameObject newRoot = Instantiate(rootPrefab, parent);
        LineRenderer rootRenderer = newRoot.GetComponent<LineRenderer>();

        var points = new Vector3[2];
        points[0] = previewStartPoint.Value;
        points[1] = previewEndPoint.Value;

        var distance = Vector3.Distance(points[0], points[1]);
        var widthStart = Mathf.Min(distance * rootWidthGrowthPerMeter, maxRootWidth);
        var widthEnd = Mathf.Min(0, maxRootWidth);

        rootRenderer.positionCount = 2;
        rootRenderer.startWidth = widthStart;
        rootRenderer.endWidth = widthEnd;
        rootRenderer.SetPositions(points);

        LineRenderer parentRootLR = parent.gameObject.GetComponent<LineRenderer>();

        if (parentRootLR != null)
        {
            parentRootLR.endWidth = widthStart;
        }

        rootSegments.Add(newRoot);

        // clear preview inputs for next inputs
        previewStartPoint = null;
        previewEndPoint = null;
    }
}
