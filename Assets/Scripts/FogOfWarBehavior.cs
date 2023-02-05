using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FogOfWarBehavior : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField]
    private Vector2 fallSpeed = new Vector2(0, 1);
    private bool falling = false;
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (falling)
        {
            Vector3 tmp = Helpers.CopyV3(this.transform.position);
            tmp -= (Vector3)fallSpeed * Time.deltaTime;
            fallSpeed -= (Vector2)Physics.gravity * Time.deltaTime;

            this.transform.position = tmp;
        }
    }

    public void Kill()
    {
        falling = true;
        fallSpeed = new Vector2(Random.Range(-5,5), Random.Range(-10,0));
        Destroy(this.gameObject, 5.0f);
    }
}
