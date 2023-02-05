using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseUpgradeVisual_Script : MonoBehaviour
{

    private RectTransform rTransform;

    private DefenseUpgradeVisualIcon_Script[] mesIconesCurrentementExistants;

    [SerializeField]
    private GameObject prefabIcon;
    // Start is called before the first frame update
    void Start()
    {
        rTransform = GetComponent<RectTransform>();
    }

    public void InitIcons(int nbIcones) {
        mesIconesCurrentementExistants = new DefenseUpgradeVisualIcon_Script[nbIcones];
        if(rTransform == null) rTransform = GetComponent<RectTransform>(); 
        rTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, nbIcones / 5 * 64 + 6);
        rTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Clamp(nbIcones ,0,5) * 64 + 6);
        for (int i = 0; i < nbIcones; i++) {
            GameObject go = Instantiate(prefabIcon, transform);
            mesIconesCurrentementExistants[i] = go.GetComponent<DefenseUpgradeVisualIcon_Script>();
        }
    }

    public void SetAssignedDefenses(GameObject[] defenses) {
        for (int i = 0; i < defenses.Length; i++) {
            mesIconesCurrentementExistants[i].SetAssignedDefense(defenses[i]);
        }
    }
    public void SetSingleAssignedDefense(GameObject go,int id) {
        mesIconesCurrentementExistants[id].SetAssignedDefense(go);
    }

}
