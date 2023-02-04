using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D),typeof(CircleCollider2D))]
public class Projectile_Script : MonoBehaviour{

    private DefenseStats defenseStats;
    private Rigidbody2D rb;
    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }
    public void Preshot(Enemy_Script enemy,DefenseStats def) {
        defenseStats = def;
        Enemy_Script enemy_Script = enemy.GetComponent<Enemy_Script>();
        Vector2 vel = enemy_Script.GetVelocity();
        Vector2 pos = enemy.transform.position;
        Vector2 myPos = transform.position;

        float myVel = def.projectileSpeed;

        Vector2 relativePos = pos - myPos;
        float timeToGo = relativePos.magnitude / myVel;

        Vector2 newPos = pos + vel * timeToGo;

        Vector2 relativePos2 = newPos - myPos;
        float timeToGo2 = relativePos2.magnitude / myVel;

        Vector2 perfectSpeed = Vector2.Lerp(relativePos, relativePos2, timeToGo2 / timeToGo);
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        rb.velocity = perfectSpeed.normalized* myVel;
        StartCoroutine(AutoDestroy());
    }

    IEnumerator AutoDestroy() {
        yield return new WaitForSeconds(defenseStats.fightRange / defenseStats.projectileSpeed);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Enemy")) {
            collision.gameObject.GetComponent<Enemy_Script>().ReceiveDamage(defenseStats.damage);
            Destroy(this.gameObject);
        }
    }
}