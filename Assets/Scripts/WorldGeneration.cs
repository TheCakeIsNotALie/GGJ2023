using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGeneration : MonoBehaviour
{
    public GameObject waterPocketPrefab;
    public int numberOfPockets = 5;
    public float startingSize = 2.5f;
    public float variation = 2;
    public Rect spawnZone;
    public float zPosition;

    List<KeyValuePair<float, Vector3>> spawns = new List<KeyValuePair<float, Vector3>>();
    List<GameObject> waterPocketList = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
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
                float y = Random.Range(spawnZone.y, spawnZone.y + spawnZone.height);
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

            var wpb = waterPocket.GetComponent<WaterPocketBehavior>();
            wpb.segments = 20;
            wpb.width = startingSize + Random.Range(-variation, variation);
            wpb.height = startingSize + Random.Range(-variation, variation);

            spawns.Add(new KeyValuePair<float, Vector3>(Mathf.Max(wpb.width, wpb.height) + 4, position));
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void SpawnWaterPocket()
    {

    }
}
