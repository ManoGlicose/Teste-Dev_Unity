using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    [Header("Components")] // Componentes necessários para o funcionamento do script
    Rigidbody2D rb;

    [Header("Movement")]
    public float projectileSpeed = 18; // A velocidade do projétil

    [HideInInspector] public bool isFromPlayer = true; // Booleana que me diz se o projétil foi disparado pelo player ou não

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        Destroy(gameObject, 10); // Destrói o projétil depois de 10 segundos
    }

    private void FixedUpdate()
    {
        rb.velocity = transform.right * projectileSpeed; // Move o projétil 
    }

    private void OnTriggerEnter2D(Collider2D collision) // Se encontrar o inimigo, aplica dano à ele. Mesma coisa com o player
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
