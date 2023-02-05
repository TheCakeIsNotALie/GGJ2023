using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GameState { Seed, Sapling, YoungTree, OldTree, FullGrown }

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private float timeElapsedSinceStart = 0f;
    [Header("Game state")]
    [SerializeField]
    private Animator cameraAnimator;
    [SerializeField]
    private GameObject undergroundCameraTarget;
    [SerializeField]
    private float cameraSpeed = 1;
    [SerializeField]
    private RootController rootController;
    private bool overgroundCamera = true;

    [Header("Growth Settings")]
    [SerializeField]
    private float timeBeforeSapling = 10f;
    [SerializeField]
    private float timeBeforeYoungTree = 120f;
    [SerializeField]
    private float timeBeforeOldTree = 300f;
    [SerializeField]
    private float timeBeforeFullGrown = 500f;
    [SerializeField]
    private GrowthVisual_Script growthVisual;

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

    [Header("Tree Settings")]
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
        growthVisual.SetTicks(timeBeforeSapling, timeBeforeYoungTree, timeBeforeOldTree, timeBeforeFullGrown);
        growthVisual.SetGrowth(timeElapsedSinceStart, timeBeforeFullGrown);
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
        float treeGrowthPercent = Mathf.InverseLerp(timeBeforeSapling, timeBeforeFullGrown, timeElapsedSinceStart);
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
                if (timeElapsedSinceStart > timeBeforeSapling) {
                    actualState = GameState.Sapling;
                }
                break;
            case GameState.Sapling:
                if (timeElapsedSinceStart > timeBeforeYoungTree) {
                    actualState = GameState.YoungTree;
                }
                break;
            case GameState.YoungTree:
                if (timeElapsedSinceStart > timeBeforeOldTree) {
                    actualState = GameState.OldTree;
                }
                break;
            case GameState.OldTree:
                if (timeElapsedSinceStart > timeBeforeFullGrown) {
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
        Time.timeScale = debugTime;

        timeElapsedSinceStart += Time.deltaTime;

        // camera controls
        if (Input.GetKeyDown("space"))
        {
            // switch state between over and under ground
            overgroundCamera = !overgroundCamera;
            if (overgroundCamera)
            {
                cameraAnimator.Play("Overground");
            }
            else
            {
                cameraAnimator.Play("Underground");
            }

            rootController.hasFocus = !overgroundCamera;
        }
        // if we are underground with the camera
        if (!overgroundCamera)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            undergroundCameraTarget.transform.position += new Vector3(cameraSpeed*horizontalInput, cameraSpeed*verticalInput);
        }

        if (lose) {
            return;
        }

        //Show Growth
        growthVisual.SetGrowth(timeElapsedSinceStart, timeBeforeFullGrown);

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
