using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GameState { Seed, Sapling, YoungTree, OldTree, FullGrown }

public class GameManager : MonoBehaviour
{
    private float elapsedTime = 0f;

    [SerializeField]
    private float timeBeforeSapling = 10f;
    [SerializeField]
    private float timeBeforeYoungTree = 120f;
    [SerializeField]
    private float timeBeforeOldTree = 300f;
    [SerializeField]
    private float timeBeforeFullGrown = 500f;

    private GameState actualState = GameState.Seed;

    [SerializeField]
    Tree_Script tree;

    void Start() {
        Time.timeScale = 20;    
    }
    // Update is called once per frame
    void Update()
    {
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
        float percent = Mathf.InverseLerp(timeBeforeSapling, timeBeforeFullGrown, elapsedTime);
        tree.SetSize(percent);

    }
}
