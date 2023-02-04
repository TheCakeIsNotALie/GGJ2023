using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EnemyState { WaitingOrder, Moving, Attacking, Dying }


[RequireComponent(typeof(Rigidbody2D))]
public class Enemy_Script : MonoBehaviour
{


    [SerializeField]
    private EnemyStats stats;

    [SerializeField]
    private GameManager target;

    private EnemyState state = EnemyState.WaitingOrder;
    public EnemyState State { get => state; }

    private bool hasEnemy = false;
    private bool hasTarget = false;


    private float attackDelay = 0f;
    private float currentPv = 0f;

    private Rigidbody2D rb;

    public void SetEnemy(EnemyStats anEnemy) {
        stats = anEnemy;
        currentPv = anEnemy.pv;
        GetComponent<SpriteRenderer>().sprite = stats.image;
        hasEnemy = true;
    }
    public void SetTarget(GameManager aTarget) {
        target = aTarget;
        hasTarget = true;
    }

    public Vector2 GetVelocity() {
        if(rb == null) {
            rb = GetComponent<Rigidbody2D>();
        }
        return rb.velocity;
    }

    public void ReceiveDamage(float value) {
        currentPv -= value;
        if(currentPv <= 0) {
            rb.velocity = Vector2.zero;
            GetComponent<Collider2D>().enabled = false;
            state = EnemyState.Dying;
        }
    }

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        if(target != null && stats != null) {
            hasEnemy = true;
            hasTarget = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasEnemy || !hasTarget) {
            return;
        }
        Vector2 myPos = transform.position;
        switch (state) {
            case EnemyState.WaitingOrder:
                
                if (myPos.magnitude > stats.attackDistance) {
                    
                    Vector2 speed = target.transform.position - transform.position;
                    speed.Normalize();
                    speed *= stats.speed;
                    rb.velocity = speed;

                    state = EnemyState.Moving;
                } else {
                    rb.velocity = Vector2.zero;
                    state = EnemyState.Attacking;
                }
                break;
            case EnemyState.Moving:
                if (myPos.magnitude < stats.attackDistance) {
                    rb.velocity = Vector2.zero;
                    state = EnemyState.Attacking;
                } else {
                    Vector2 speed = target.transform.position - transform.position;
                    speed.Normalize();
                    speed *= stats.speed;
                    rb.velocity = speed;
                }
                break;
            case EnemyState.Attacking:
                attackDelay -= Time.deltaTime;
                if(attackDelay <= 0) {
                    target.Attacked(stats.damage);
                    attackDelay = stats.timeBetweenAttacks;
                }
                break;
            case EnemyState.Dying:
                target.enemies.Remove(this);
                Destroy(gameObject);
                break;
            default:
                break;
        }
    }
}
