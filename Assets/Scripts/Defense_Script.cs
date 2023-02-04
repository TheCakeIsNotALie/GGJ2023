using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defense_Script : MonoBehaviour
{
    [SerializeField]
    private DefenseStats defensesStats = null;

    private GameManager gm;
    private SpriteRenderer sr;
    private float currentTimerBeforeAttack = 0f;


    [SerializeField]
    private GameObject prefabProjectile;
    private Enemy_Script target = null;

    public void SetDefenseStats(DefenseStats def) {
        if(sr == null) sr = GetComponent<SpriteRenderer>();
        defensesStats = def;
        sr.sprite = def.image;
        currentTimerBeforeAttack = def.timeBetweenAttacks;
    }
    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        gm = GameManager.instance;
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null && target.State == EnemyState.Dying) {
            target = null;
        }
        if(target == null) {
            foreach (Enemy_Script enemy in gm.enemies) {
                if(Vector2.Distance(enemy.transform.position,this.transform.position)<= defensesStats.fightRange && enemy.State != EnemyState.Dying) {
                    target = enemy;
                    break;
                }
            }
        }
        //Si c'est toujours null
        if(target == null) {
            return;
        }
        
        currentTimerBeforeAttack -= Time.deltaTime;
        if (currentTimerBeforeAttack > 0)
            return;
        currentTimerBeforeAttack = defensesStats.timeBetweenAttacks;

        GameObject projectile = Instantiate(prefabProjectile);
        projectile.transform.position = transform.position;
        Projectile_Script projectile_Script = projectile.GetComponent<Projectile_Script>();
        projectile_Script.Preshot(target,defensesStats);
    }

    private void OnDrawGizmos() {
        if(defensesStats != null) {

            Gizmos.DrawWireSphere(transform.position, defensesStats.fightRange);
        }
    }
}
