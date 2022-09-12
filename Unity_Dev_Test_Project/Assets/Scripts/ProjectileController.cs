using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [Header("Components")]
    Rigidbody2D rb;

    [Header("Movement")]
    public float projectileSpeed = 18;

    [HideInInspector] public bool isFromPlayer = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        Destroy(gameObject, 10);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        rb.velocity = transform.right * projectileSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<EnemyController>())
        {
            if (isFromPlayer)
            {
                collision.GetComponent<EnemyController>().TakeDamage(20);
                print("HIT ENEMY");
                Destroy(gameObject);
            }
        }

        if (collision.GetComponent<PlayerController>())
        {
            if (!isFromPlayer)
            {
                collision.GetComponent<PlayerController>().TakeDamage(10);
                print("HIT PLAYER");
                Destroy(gameObject);
            }
        }
    }
}
