using System;
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

    private float currentGrowth = 0f;


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
    [SerializeField]
    private float sapPerSecondPerGrowth = 9f;


    public float previewCost = 0;

    [Header("Tree Settings")]
    [SerializeField]
    Tree_Script tree;
    [SerializeField]
    private float hydratationMaxNeededPercent = 0.7f;
    [SerializeField]
    private float hydratationBase = 0.1f;
    private float hydratationActual = 0.1f;
    private float hydratationNeededActual = 0f;
    public float hydratationMaxPossible = 0f;
    [SerializeField]
    private HydratationVisual_Script hydratationVisual;

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
    public List<Enemy_Script> enemies = new();



    [Header("Defenses Settings")]
    [SerializeField]
    private GameObject prefabDefense;
    [SerializeField]
    private Transform[] defensePoints;
    private GameObject[] defenses = new GameObject[0];

    [SerializeField]
    private DefenseUpgradeVisual_Script defenseUpgrade;


    [Header("Lose UI")]
    [SerializeField]
    private GameObject loseUI;


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
        growthVisual.SetGrowth(timeElapsedSinceStart, timeBeforeFullGrown,true);
        defenses = new GameObject[defensePoints.Length];
        defenseUpgrade.InitIcons(defensePoints.Length);
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
    public int HasSpaceForDefense() {
        for (int i = 0; i < defenses.Length; i++) {
            if(defenses[i] == null) {
                return i;
            }
        }
        return -1;
    }
    public void DeleteDefense(Defense_Script def) {
        int position = def.defensePosition;
        Destroy(def.gameObject);
        defenses[position] = null;
    }
    public bool Buy(DefenseStats defenseStats) {
        int id = HasSpaceForDefense();
        if (id == -1) {
            return false;
        }
        if (!Buy(defenseStats.cost)) {
            return false;
        }
        PutDefenseAtPoint(defenseStats, id);
        return true;
    }
    private void PutDefenseAtPoint(DefenseStats def,int id) {
        Transform parent = defensePoints[id];
        GameObject go = Instantiate(prefabDefense, parent);
        Defense_Script defScript = go.GetComponent<Defense_Script>();
        defScript.SetDefenseStats(def,id);
        defenses[id] = go;
        defenseUpgrade.SetSingleAssignedDefense(go, id);
        print("Put defense " + def.name + " At "+id.ToString());
    }

    private void Lose() {
        lose = true;
        loseUI.SetActive(true);
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
        sapVisual.PreviewCost(previewCost, maxSap, currentSap >= previewCost);
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

    private void CalculateHydratation() {
        hydratationNeededActual = hydratationMaxNeededPercent * currentGrowth * hydratationMaxPossible;

        hydratationActual = hydratationBase;
        HashSet<WaterPocketBehaviour> connectedWaterPockets =  rootController.GetConnectedWaterPockets();
        WorldGeneration wg = GetComponent<WorldGeneration>();
        List<GameObject> waterPockets = wg.waterPocketList;
        foreach (GameObject go in waterPockets) {
            WaterPocketBehaviour WaterPocketBehaviour = go.GetComponent<WaterPocketBehaviour>();
            if (connectedWaterPockets.Contains(WaterPocketBehaviour)) {
                hydratationActual += WaterPocketBehaviour.waterQuantity;
            }
        }

        hydratationVisual.SetValue(hydratationActual, hydratationNeededActual,hydratationMaxPossible);

    }
    // Update is called once per frame
    void Update()
    {
        Time.timeScale = debugTime;

        if(hydratationActual >= hydratationNeededActual) {
            timeElapsedSinceStart += Time.deltaTime;
            currentGrowth = timeElapsedSinceStart / timeBeforeFullGrown;
        }

        // camera controls
        if (Input.GetKeyDown(KeyCode.Space))
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
        growthVisual.SetGrowth(timeElapsedSinceStart, timeBeforeFullGrown,hydratationActual >= hydratationNeededActual);

        //Hydratation Stuff
        CalculateHydratation();

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
