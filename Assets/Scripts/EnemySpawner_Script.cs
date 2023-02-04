using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner_Script : MonoBehaviour
{
    [SerializeField]
    private EnemyStats[] enemies;


    [SerializeField]
    private Transform[] spawnPoints;

    [SerializeField]
    private GameObject enemyPrefab;

    public Enemy_Script SpawnAnEnemy(GameManager gm,GameState actual) {
        List<EnemyStats> possible = new List<EnemyStats>();
        for (int i = 0; i < enemies.Length; i++) {
            if(enemies[i].spawnTime <= actual) {
                possible.Add(enemies[i]);
            }
        }
        if(possible.Count == 0) {
            return null;
        }

        EnemyStats rnd = possible[Random.Range(0, possible.Count)];
        Transform point = spawnPoints[Random.Range(0, spawnPoints.Length)];

        GameObject go = Instantiate(enemyPrefab, point);
        Enemy_Script enemy = go.GetComponent<Enemy_Script>();
        enemy.SetEnemy(rnd);
        enemy.SetTarget(gm);
        return enemy;
    }
}
