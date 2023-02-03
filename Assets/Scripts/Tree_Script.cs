using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(LineRenderer))]
public class Tree_Script : MonoBehaviour {
    [SerializeField]
    private float maxTrunkWidth = 2f;
    [SerializeField]
    private float maxTrunkHeight = 10f;
    [SerializeField]
    private LineRenderer lr;
        

    [SerializeField]
    private Transform leavesParent;


    // Start is called before the first frame update
    void Start()
    {
        if(lr == null)
            lr = GetComponent<LineRenderer>();
    }

    public void SetSize(float percent) {

        //width
        float curveValue = percent * maxTrunkWidth;
        lr.widthMultiplier = curveValue;
        //length

        Vector3[] positions = new Vector3[lr.positionCount];
        float deltaPos = percent / (lr.positionCount-1) * maxTrunkHeight;
        for (int i = 1; i < lr.positionCount; i++) {
            positions[i] = Vector3.up * (i * deltaPos);
        }
        lr.SetPositions(positions);

        //leaves
        leavesParent.localScale = Vector3.one * percent;
        leavesParent.localPosition = Vector3.up * percent * maxTrunkHeight;
    }
}
