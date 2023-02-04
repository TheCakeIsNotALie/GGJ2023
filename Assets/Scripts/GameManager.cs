using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GameState { Seed, Sapling, YoungTree, OldTree, FullGrown }

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

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

    [Header("Enemy Settings")]
    [SerializeField]
    private EnemySpawner_Script spawner;
    [SerializeField]
    private float timerBetweenEnemy = 5f;
    [SerializeField]
    private float timeRemainingBeforeNewEnemy = 10f;
    [SerializeField]
    private float accelerationOfEnemySpawn = 0.1f;
    [SerializeField]
    private float minimalTimeBetweenSpawn = 1f;
    public List<Enemy_Script> enemies = new List<Enemy_Script>();



    [Header("Defenses Settings")]
    [SerializeField]
    private GameObject prefabDefense;
    [SerializeField]
    private Transform[] defensePoints;
    private int takenDefensePoints;



    [Header("Debug")]
    [SerializeField, Range(1, 100)]
    private float debugTime = 1f;


    [Header("Win/Lose Condition")]
    private bool lose = false;


    private void Awake() {
        if(instance != null) {
            print("Plusieurs GameManager");
        }
        instance = this;
    }

    void Start() {
    }

    public bool CanBuy(float sapCost) {
        if (currentSap >= sapCost) {
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
    public bool HasSpaceForDefense() {
        return (takenDefensePoints < defensePoints.Length);
    }
    public bool Buy(DefenseStats defenseStats) {
        if (!HasSpaceForDefense()) {
            return false;
        }
        if (!Buy(defenseStats.cost)) {
            return false;
        }
        PutDefenseAtPoint(defenseStats, takenDefensePoints);
        takenDefensePoints++;
        return true;
    }
    private void PutDefenseAtPoint(DefenseStats def,int id) {
        Transform parent = defensePoints[id];
        GameObject go = Instantiate(prefabDefense, parent);
        Defense_Script defScript = go.GetComponent<Defense_Script>();
        defScript.SetDefenseStats(def);
        print("Put defense " + def.name + " At "+id.ToString());
    }

    private void Lose() {
        lose = false;
    }
    public void Attacked(float value) {
        currentSap -= value;
        if(currentSap <= 0) {
            Lose();
        }
    }




    private void ActualizeTree(float deltaTime) {
        float treeGrowthPercent = Mathf.InverseLerp(timeBeforeSapling, timeBeforeFullGrown, elapsedTime);
        tree.SetSize(treeGrowthPercent);

        //Sap Gain
        currentSap += sapPerSecond * deltaTime;
        if (currentSap > maxSap) {
            currentSap = maxSap;
        }
        sapVisual.ChangeValue(currentSap, maxSap);
        sapVisual.PreviewCost(nextCost, maxSap, currentSap >= nextCost);
    }
    private void ActualizeState() {
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
    }
    // Update is called once per frame
    void Update()
    {
        if (lose) {
            return;
        }
        Time.timeScale = debugTime;


        elapsedTime += Time.deltaTime;
        //State Managment
        ActualizeState();

        //Actualize Tree
        ActualizeTree(Time.deltaTime);

        //EnemySpawn
        EnemySpawn(Time.deltaTime);
    }
    private void EnemySpawn(float deltaTime) {
        timeRemainingBeforeNewEnemy -= deltaTime;
        if(timeRemainingBeforeNewEnemy <= 0) {
            enemies.Add(spawner.SpawnAnEnemy(this, actualState));
            timeRemainingBeforeNewEnemy = timerBetweenEnemy;
            timerBetweenEnemy -= accelerationOfEnemySpawn;
            if (timerBetweenEnemy < minimalTimeBetweenSpawn) {
                timerBetweenEnemy = minimalTimeBetweenSpawn;
                accelerationOfEnemySpawn = 0f;
            }
        }
    }
}
