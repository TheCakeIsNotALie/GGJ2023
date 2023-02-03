using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GameState { Seed, Sapling, YoungTree, OldTree, FullGrown }

public class GameManager : MonoBehaviour
{
    private float elapsedTime = 0f;
    [Header("GameState changing")]
    [SerializeField]
    private float timeBeforeSapling = 10f;
    [SerializeField]
    private float timeBeforeYoungTree = 120f;
    [SerializeField]
    private float timeBeforeOldTree = 300f;
    [SerializeField]
    private float timeBeforeFullGrown = 500f;

    private GameState actualState = GameState.Seed;


    [Header("Sap attributes")]
    [SerializeField]
    SapVisual_Script sapVisual;
    private float currentSap = 0f;
    [SerializeField]
    private float maxSap = 50f;
    [SerializeField]
    private float sapPerSecond = 1f;

    public float nextCost = 0;

    [Header("Tree")]
    [SerializeField]
    Tree_Script tree;



    [Header("Debug")]
    [SerializeField, Range(1, 100)]
    private float debugTime = 1f;
    void Start() {
    }

    public bool CanBuy(float sapCost) {
        if (currentSap > sapCost) {
            return true;
        }
        return false;
    }
    public bool Buy(float sapCost) {
        bool ret = CanBuy(sapCost);
        if (ret) {
            currentSap -= sapCost;
        }
        return ret;
    }

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = debugTime;


        elapsedTime += Time.deltaTime;
        //State Managment
        switch (actualState) {
            case GameState.Seed:
                if (elapsedTime > timeBeforeSapling) {
                    actualState = GameState.Sapling;
                }
                break;
            case GameState.Sapling:
                if (elapsedTime > timeBeforeYoungTree) {
                    actualState = GameState.YoungTree;
                }
                break;
            case GameState.YoungTree:
                if (elapsedTime > timeBeforeOldTree) {
                    actualState = GameState.OldTree;
                }
                break;
            case GameState.OldTree:
                if (elapsedTime > timeBeforeFullGrown) {
                    actualState = GameState.FullGrown;
                }
                break;
            case GameState.FullGrown:
            default:
                break;
        }


        //Actualize Tree
        float treeGrowthPercent = Mathf.InverseLerp(timeBeforeSapling, timeBeforeFullGrown, elapsedTime);
        tree.SetSize(treeGrowthPercent);

        //Sap Gain
        currentSap += sapPerSecond * Time.deltaTime;
        if (currentSap > maxSap) {
            currentSap = maxSap;
        }
        sapVisual.ChangeValue(currentSap, maxSap);
        sapVisual.PreviewCost(nextCost, maxSap,currentSap >= nextCost);
    }
}
