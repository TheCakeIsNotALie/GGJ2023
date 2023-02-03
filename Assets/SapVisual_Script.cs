using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SapVisual_Script : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI textOutput;

    [SerializeField]
    private RectTransform sapValue;
    [SerializeField]
    private RectTransform previewCost;


    private RectTransform rTransform;
    private float maxHeight = 0;

    private void Start() {
        rTransform = GetComponent<RectTransform>();
        maxHeight = rTransform.rect.height - 10;
    }

    public void ChangeValue(float value,float maxValue) {
        textOutput.text = string.Format("Sap\n{0:000}/{1}", Mathf.FloorToInt(value), maxValue);

        float newHeight = maxHeight * (value / maxValue);

        sapValue.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
    }



    public void PreviewCost(float cost,float maxValue,bool canAfford) {
        float newPosY = sapValue.rect.height + 5;
        float newHeight = maxHeight * (cost / maxValue);
        if(canAfford) {
            previewCost.GetComponent<Image>().color = new Color(0,0,0, .5f);
        } else {
            previewCost.GetComponent<Image>().color = new Color(1f, 0, 0, .5f);
        }
        previewCost.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rTransform.rect.width - 10);
        previewCost.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
        previewCost.SetLocalPositionAndRotation(new Vector3(-(previewCost.rect.width/2f+5), newPosY), new Quaternion());
    }
}
