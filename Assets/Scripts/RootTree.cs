using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RootTree : MonoBehaviour
{
    public class Node
    {
        public Vector2 pos;
        private Node parent;
        private Node left;
        private Node right;

        public Node Parent { get => parent; set => parent = value; }
        public Node Left { get => left; set => left = value; }
        public Node Right { get => right; set => right = value; }

        public Node()
        {
            this.pos = Vector2.zero;
        }

        public Node(Vector2 pos, Node parent, Node left, Node right)
        {
            this.pos = pos;
            this.parent = parent;
            this.left = left;
            this.right = right;
        }

        public override int GetHashCode()
        {
            int hash = this.pos.GetHashCode();
            if(this.left != null)
            {
                hash += this.GetHashCode();
            }
            if (this.right != null)
            {
                hash += this.GetHashCode();
            }
            return hash;
        }
    }

    private int previousHash = 0;
    public Node main = new Node();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(previousHash != main.GetHashCode())
        {
            previousHash = main.GetHashCode();


        }
    }


}
