using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RootTree : MonoBehaviour
{
    public class RootNode
    {
        public static GameObject rootPrefab;
        public GameObject self;
        private RootNode parent;
        private RootNode left;
        private RootNode right;
        private int childCount = 0;

        public RootNode Parent
        {
            get => parent;
            set
            {
                parent = value;
            }
        }
        public RootNode Left
        {
            get => left;
            set
            {
                left = value;
            }
        }
        public RootNode Right
        {
            get => right;
            set
            {
                right = value;
            }
        }
        public int ChildCount { get => childCount; set => childCount = value; }

        public RootNode(Vector3 pos)
        {
            this.self = Instantiate(rootPrefab);
            this.self.transform.position = pos;
        }

        public RootNode(Vector3 pos, RootNode parent, RootNode left = null, RootNode right = null)
        {
            this.self = Instantiate(rootPrefab, parent?.self.transform);
            this.self.transform.position = pos;
            this.parent = parent;
            this.Left = left;
            this.Right = right;
        }

        public void UpdateChildCount()
        {
            this.childCount = 0;
            if (left != null)
            {
                left.UpdateChildCount();
                this.childCount += left.ChildCount + 1;
            }
            if (right != null)
            {
                right.UpdateChildCount();
                this.childCount += right.ChildCount + 1;
            }
        }
        public override int GetHashCode()
        {
            int hash = this.self.transform.position.GetHashCode();
            if (this.left != null)
            {
                hash += this.left.GetHashCode();
            }
            if (this.right != null)
            {
                hash += this.right.GetHashCode();
            }
            return hash;
        }
    }

    public GameObject rootPrefab;
    [SerializeField]
    private WorldGeneration worldGen;
    public float zPosition = -1;
    public float minWidth = 0.1f;
    public float maxWidth = 1;
    public float rootWidthGrowthPerChild = 0.125f;

    private int previousHash = 0;
    public RootNode main;

    private List<Collider2D> waterPocketColliders;
    public HashSet<WaterPocketBehaviour> linkedWaterPockets = new HashSet<WaterPocketBehaviour>();

    // Start is called before the first frame update
    void Start()
    {
        RootNode.rootPrefab = rootPrefab;
        main = new RootNode(new Vector3(0, 0, zPosition));
        UpdateNodeDisplay(main);
    }

    // Update is called once per frame
    void Update()
    {
        // only update if tree has changed
        if (previousHash != main?.GetHashCode())
        {
            previousHash = main.GetHashCode();

            main.UpdateChildCount();
            UpdateNodeDisplay(main);
            CheckForResourcesCollisions();
        }
    }

    //private void OnDrawGizmos()
    //{
    //    if (Application.isPlaying)
    //    {
    //        DrawGizmo(main);
    //    }
    //}

    //void DrawGizmo(RootNode node)
    //{
    //    int count = 0;
    //    var parentNode = node.Parent;
    //    while(parentNode != null)
    //    {
    //        count++;
    //        parentNode = parentNode.Parent;
    //    }
    //    if(node.Parent != null && node.Parent.Left == node)
    //    {
    //        Handles.Label(node.self.transform.position, count + "L" + node.ChildCount + "C");
    //    }
    //    if (node.Parent != null && node.Parent.Right == node)
    //    {
    //        Handles.Label(node.self.transform.position, count + "R" + node.ChildCount + "C");
    //    }

    //    if(node.Right != null)
    //    {
    //        DrawGizmo(node.Right);
    //    }
    //    if(node.Left != null)
    //    {
    //        DrawGizmo(node.Left);
    //    }
    //}

    public void CheckForResourcesCollisions()
    {
        FindCollision(main);
    }

    private void FindCollision(RootNode node)
    {
        if (node.Left != null)
        {
            FindCollision(node.Left);
        }
        if (node.Right != null)
        {
            FindCollision(node.Right);
        }

        var parentNode = node.Parent != null ? node.Parent : node;
        List<RaycastHit2D> hits = new List<RaycastHit2D>();
        Physics2D.Linecast(parentNode.self.transform.position, node.self.transform.position, new ContactFilter2D(), hits);

        foreach (var hit in hits)
        {
            var wpb = hit.collider.gameObject.GetComponent<WaterPocketBehaviour>();
            if (wpb != null)
            {
                linkedWaterPockets.Add(wpb);
                continue;
            }
            var fowb = hit.collider.gameObject.GetComponent<FogOfWarBehaviour>();
            if (fowb != null)
            {
                fowb.Kill();
            }
        }
    }

    /// <summary>
    /// Recursively updates the line renderers based on current tree state
    /// </summary>
    public void UpdateNodeDisplay(RootNode node)
    {
        var lr = node.self.GetComponent<LineRenderer>();

        // draw 
        var parentNode = node.Parent != null ? node.Parent : node;
        var points = new Vector3[2];
        points[0] = parentNode.self.transform.position;
        points[1] = node.self.transform.position;

        var distance = Vector3.Distance(points[0], points[1]);
        float widthStart = Mathf.Clamp((node.ChildCount + 1) * rootWidthGrowthPerChild, minWidth, maxWidth);
        float widthEnd = Mathf.Clamp(node.ChildCount * rootWidthGrowthPerChild, minWidth, maxWidth);

        lr.positionCount = 2;
        lr.startWidth = widthStart;
        lr.endWidth = widthEnd;
        lr.SetPositions(points);

        if (node.Left != null)
        {
            UpdateNodeDisplay(node.Left);
        }
        if (node.Right != null)
        {
            UpdateNodeDisplay(node.Right);
        }
    }

    public void InsertIntersection(RootNode insert, RootNode before, RootNode after)
    {
        if (before.Left == after)
        {
            insert.Left = before.Left;
            before.Left = insert;
            insert.Parent = before;
            if (insert.Left != null)
            {
                insert.Left.Parent = insert;
            }
        }
        else
        {
            insert.Right = before.Right;
            before.Right = insert;
            insert.Parent = before;
            if (insert.Right != null)
            {
                insert.Right.Parent = insert;
            }
        }
    }

    public void InsertLeaf(RootNode insert, RootNode parent)
    {
        if (parent.Left == null)
        {
            parent.Left = insert;
            insert.Parent = parent;
        }
        else if (parent.Right == null)
        {
            parent.Right = insert;
            insert.Parent = parent;
        }
        else
        {
            print("No available leaf, skipping");
            return;
        }

        UpdateNodeDisplay(insert);
        UpdateNodeDisplay(parent);
    }

    public class PointSearch
    {
        public RootNode before;
        public RootNode after;
        public bool left;
        public float distance;
        public Vector3 pointOnSegment;

        public PointSearch(RootNode before, RootNode after, bool left, float distance, Vector3 pointOnSegment)
        {
            this.before = before;
            this.after = after;
            this.left = left;
            this.distance = distance;
            this.pointOnSegment = pointOnSegment;
        }
    }

    public PointSearch ClosestPointOnTree(Vector3 point)
    {
        return FindClosestPoint(main, point);
    }
    /// <summary>
    /// Maths are made using 2d distance to avoid Z issues
    /// </summary>
    /// <param name="node"></param>
    /// <param name="point"></param>
    /// <returns></returns>
    private PointSearch FindClosestPoint(RootNode node, Vector3 point)
    {
        //print("Node : " + node + ", Node parent : " + node.Parent);
        PointSearch returnValue;
        var parentNode = node.Parent != null ? node.Parent : node;
        var start = parentNode.self.transform.position;
        var end = node.self.transform.position;
        var nearestPointOnSegment = MathHelper.NearestPointOnSegment(point, start, end);

        returnValue = new PointSearch(parentNode, node, true, Vector2.Distance(point, nearestPointOnSegment), nearestPointOnSegment);

        if (node.Left != null)
        {
            var nearestLeft = FindClosestPoint(node.Left, point);
            if (nearestLeft.distance < returnValue.distance)
            {
                returnValue = nearestLeft;
            }
        }

        if (node.Right != null)
        {
            var nearestRight = FindClosestPoint(node.Right, point);
            if (nearestRight.distance < returnValue.distance)
            {
                returnValue = nearestRight;
                returnValue.left = false;
            }
        }

        return returnValue;
    }
}
