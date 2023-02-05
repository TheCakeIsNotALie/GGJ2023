using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HydratationVisual_Script : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI textOutput;

    [SerializeField]
    private RectTransform rValue;
    [SerializeField]
    private RectTransform rNeededValue;
    [SerializeField]
    private GameObject goNeededValue;

    [SerializeField]
    private Color cPositive = Color.blue;
    [SerializeField]
    private Color cNegative = Color.red;




    private RectTransform rTransform;
    private float maxHeight = 0;

    private void Start() {
        rTransform = GetComponent<RectTransform>();
        maxHeight = rTransform.rect.height - 10;
    }

    public void SetValue(float value,float neededValue,float maxPossible) {
        textOutput.text = string.Format("Water\n{0}/{1}", Mathf.RoundToInt(value),Mathf.RoundToInt(maxPossible));

        if (neededValue > maxPossible)
            neededValue = maxPossible;
        if (value > maxPossible)
            value = maxPossible;

        bool positive = value > neededValue;
        float newHeight = maxHeight * (value/maxPossible);
        float newHeightNeeded = maxHeight*(neededValue/maxPossible);
        

        rValue.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (positive?newHeightNeeded:newHeight));
        goNeededValue.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, (positive ? newHeight - newHeightNeeded : newHeightNeeded - newHeight));

        goNeededValue.GetComponent<Image>().color = (positive? cPositive : cNegative);

    }
}
