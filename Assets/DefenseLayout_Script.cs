using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseLayout_Script : MonoBehaviour
{
    [SerializeField]
    private DefenseStats[] baseDefenses;

    [SerializeField]
    private GameObject prefabIcon;


    private List<IconShower_Script> icons = new List<IconShower_Script>();


    private GameManager gm;
    private RectTransform rTransform;
    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.instance;
        rTransform = GetComponent<RectTransform>();
        rTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, baseDefenses.Length * 64 + 6);

        for (int i = 0; i < baseDefenses.Length; i++) {
            GameObject go = Instantiate(prefabIcon, transform);
            IconShower_Script iconShow = go.GetComponent<IconShower_Script>();
            iconShow.UpdateAll(baseDefenses[i]);
            icons.Add(iconShow);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}
