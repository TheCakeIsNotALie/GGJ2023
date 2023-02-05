using GK;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    [Header("Water Pockets")]
    public GameObject waterPocketPrefab;
    public float waterPocketPosMean;
    public float waterPocketPosStdDev;
    public float waterPocketMinSize = 2.5f;
    public float waterPocketMaxSize = 2.5f;
    public AnimationCurve waterPocketSizeCurve;
    public float waterPocketSizeStdDev;
    public int numberOfPockets = 5;
    public float waterDensity = 1;
    public Rect spawnZone;
    public float zPosition = -1;

    List<KeyValuePair<float, Vector3>> spawns = new List<KeyValuePair<float, Vector3>>();
    public List<GameObject> waterPocketList = new List<GameObject>();

    [SerializeField]
    private RootTree rootTree;

    [Header("Fog of War")]
    [SerializeField]
    private bool activateFogOfWar;
    [SerializeField]
    private GameObject fogOfWarPrefab;
    [SerializeField]
    private GameObject fogOfWarContainer;
    [SerializeField]
    private Color fogOfWarBaseColor;
    [SerializeField]
    private float fogOfWarVariation;
    [SerializeField]
    private int amountFogPoints = 10000;
    private Vector3[] fogPoints;
    // Start is called before the first frame update
    void Start()
    {
        if (activateFogOfWar)
        {
            GenerateFogOfWar();
        }

        SpawnWaterPockets();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GenerateFogOfWar()
    {
        fogPoints = new Vector3[amountFogPoints];
        for (int i = 0; i < amountFogPoints; i++)
        {
            fogPoints[i] = new Vector3(Random.Range(spawnZone.x, spawnZone.x + spawnZone.width), Random.Range(spawnZone.y, spawnZone.y + spawnZone.height), zPosition);
        }

        var calc = new DelaunayCalculator();

        var results = calc.CalculateTriangulation(fogPoints.Select(x => (Vector2)x).ToList());
        for (int i = 0; i < results.Triangles.Count; i += 3)
        {
            var c0 = results.Vertices[results.Triangles[i]];
            var c1 = results.Vertices[results.Triangles[i + 1]];
            var c2 = results.Vertices[results.Triangles[i + 2]];

            var mesh = new Mesh
            {
                name = "Procedural Mesh"
            };

            mesh.vertices = new Vector3[] {
                new Vector3(c0.x, c0.y, 0), new Vector3(c1.x, c1.y, 0), new Vector3(c2.x, c2.y, 0)
            };

            var instance = Instantiate(fogOfWarPrefab, fogOfWarContainer.transform);
            instance.transform.position = new Vector3(0, 0, zPosition);
            var meshRenderer = instance.GetComponent<MeshRenderer>();
            var meshFilter = instance.GetComponent<MeshFilter>();
            var polygonCollider2D = instance.GetComponent<PolygonCollider2D>();
            meshFilter.mesh = mesh;
            polygonCollider2D.SetPath(0, new Vector2[] { c0, c1, c2 });
            mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1) };
            mesh.triangles = new int[] { 2, 1, 0 };
            meshRenderer.material.color = new Color(
                fogOfWarBaseColor.r + Random.Range(-fogOfWarVariation, fogOfWarVariation),
                fogOfWarBaseColor.g + Random.Range(-fogOfWarVariation, fogOfWarVariation),
                fogOfWarBaseColor.b + Random.Range(-fogOfWarVariation, fogOfWarVariation)
                );
        }

    }

    void SpawnWaterPockets()
    {
        for (int i = 0; i < numberOfPockets; i++)
        {
            var waterPocket = Instantiate(waterPocketPrefab);
            waterPocketList.Add(waterPocket);

            Vector3 position;
            bool isTooClose;
            do
            {
                isTooClose = false;

                float x = Random.Range(spawnZone.x, spawnZone.x + spawnZone.width);
                float y = spawnZone.y + Mathf.Clamp(MathHelper.RandomStdDev(1-waterPocketPosMean, waterPocketPosStdDev), 0, 1) * spawnZone.height;
                position = new Vector3(x, y, zPosition);

                foreach (var spawn in spawns)
                {
                    if (Vector2.Distance(position, spawn.Value) < spawn.Key)
                    {
                        isTooClose = true;
                        break;
                    }
                }
            } while (isTooClose);


            waterPocket.transform.position = position;
            var normalizedDepth = 1 - (position.y - spawnZone.y) / spawnZone.height;
            var wpb = waterPocket.GetComponent<WaterPocketBehavior>();
            wpb.width = Mathf.Max(waterPocketMinSize, waterPocketMaxSize * waterPocketSizeCurve.Evaluate(normalizedDepth));
            wpb.width *= Random.Range(-0.1f, 0.1f);
            wpb.height = Mathf.Max(waterPocketMinSize, waterPocketMaxSize * waterPocketSizeCurve.Evaluate(normalizedDepth));
            wpb.height *= Random.Range(-0.1f, 0.1f);
            wpb.waterQuantity = wpb.width * wpb.height * waterDensity;
            wpb.angleOffset = Random.Range(0, Mathf.PI/2);
            wpb.segments = (int)Mathf.Max((wpb.width + wpb.height)*2, 4);

            spawns.Add(new KeyValuePair<float, Vector3>(Mathf.Max(wpb.width, wpb.height) + 4, position));
        }
    }
}
