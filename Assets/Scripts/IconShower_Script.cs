using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconShower_Script : MonoBehaviour
{
    [SerializeField]
    private Image image;
    [SerializeField]
    private GameObject filter;
    [SerializeField]
    private TMPro.TextMeshProUGUI valueText;

    private DefenseStats defenseStat;
    public void CanAfford(bool affordable) {
        filter.SetActive(!affordable || GameManager.instance.HasSpaceForDefense() == -1);
    }
    public void SetValue(int value) {
        valueText.text = string.Format("{0}", value);
    }
    public void SetSprite(Sprite sprite) {
        image.sprite = sprite;
    }
    public void UpdateAll(DefenseStats defStat) {

        SetSprite(defStat.icon);
        SetValue(Mathf.CeilToInt(defStat.cost));
        CanAfford(GameManager.instance.CanBuy(defStat.cost));
        defenseStat = defStat;
    }

    private void Update() {
        if (defenseStat == null) return;
        CanAfford(GameManager.instance.CanBuy(defenseStat.cost));
    }
    public void OnButtonClick() {
        GameManager.instance.Buy(defenseStat);
        CanAfford(GameManager.instance.CanBuy(defenseStat.cost));
    }
}
