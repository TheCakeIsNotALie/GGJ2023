using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DefenseUpgradeVisualIcon_Script : MonoBehaviour
{

    
    public GameObject assignedDefense;

    [SerializeField]
    private Image iconImage;
    [SerializeField]
    private TMPro.TextMeshProUGUI value;

    [SerializeField]
    private GameObject upgrade;
    [SerializeField]
    private GameObject delete;

    [SerializeField]
    private GameObject unaffordableFilter;

    public void OnDeleteClick() {
        print("Must Delete Something");
        Defense_Script def = assignedDefense.GetComponent<Defense_Script>();
        assignedDefense = null;
        GameManager.instance.DeleteDefense(def);
        SetVisual(null);
    }

    public void OnUpgradeClick() {
        print("Must Upgrade Something");
        
        Defense_Script def = assignedDefense.GetComponent<Defense_Script>();
        SetVisual(def.Upgrade());
    }

    public void SetAssignedDefense(GameObject go) {
        assignedDefense = go;
        SetVisual(assignedDefense.GetComponent<Defense_Script>().GetDefenseStats());
    }
    public void SetVisual(DefenseStats stats) {

        if(stats == null) {
            delete.SetActive(false);
            upgrade.SetActive(false);
            value.text = "";
            iconImage.enabled = false;
            return;
        } 
        delete.SetActive(true);

        iconImage.enabled = true;
        iconImage.sprite = stats.icon;

        if(stats.upgrade == null) {
            value.text = "";
            upgrade.SetActive(false);
        } else {
            value.text = string.Format("{0}", stats.upgrade.cost);
            upgrade.SetActive(true);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        if(assignedDefense != null) {
            Defense_Script def = assignedDefense.GetComponent<Defense_Script>();
            SetVisual(def.GetDefenseStats());
        } else {
            SetVisual(null);
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(assignedDefense == null) {
            unaffordableFilter.SetActive(false);
            return;
        }
        DefenseStats defenseStats = assignedDefense.GetComponent<Defense_Script>().GetDefenseStats();
        if(defenseStats.upgrade == null) {
            unaffordableFilter.SetActive(false);
            return;
        }

        unaffordableFilter.SetActive(!GameManager.instance.CanBuy(defenseStats.upgrade.cost));
    }
}
