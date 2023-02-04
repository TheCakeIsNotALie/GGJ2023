using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowthVisual_Script : MonoBehaviour {
    [SerializeField]
    private RectTransform p1Tick;
    [SerializeField]
    private RectTransform p2Tick;
    [SerializeField]
    private RectTransform p3Tick;
    [SerializeField]
    private RectTransform growthValue;
    private RectTransform mine;

    private void Start() {
        mine = GetComponent<RectTransform>();
    }
    public void SetGrowth(float current,float max) {
        float growthValueWidth = current / max * mine.rect.width;

        growthValue.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, growthValueWidth);
    }
    public void SetTicks(float timeBeforeP1,float timeBeforeP2, float timeBeforeP3, float timeBeforeFull) {
        float width = GetComponent<RectTransform>().rect.width;
        float p1X = timeBeforeP1 / timeBeforeFull * width;
        float p2X = timeBeforeP2 / timeBeforeFull * width;
        float p3X = timeBeforeP3 / timeBeforeFull * width;

        Vector3 ticksY = new Vector3(0, mine.position.y, 0);

        p1Tick.SetPositionAndRotation(Vector3.right*p1X + ticksY, new Quaternion());
        p2Tick.SetPositionAndRotation(Vector3.right*p2X + ticksY, new Quaternion());
        p3Tick.SetPositionAndRotation(Vector3.right*p3X + ticksY, new Quaternion());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
