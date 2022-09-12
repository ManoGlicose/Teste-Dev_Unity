using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Components")]
    Rigidbody2D rb;

    [Header("Status")]
    public float health = 100;

    [Header("Movement")]
    public float speed = 4;
    Vector3 m_velocity;
    float m_MovementSmoothing = .1f;
    Vector2 direction;

    [Header("Target")]
    public LayerMask playerLayer;
    public Transform target;

    [Header("AI Parameters")]
    public float detectionRadius = 10;
    public float safeDistance = 3;

    [Header("Animation")]
    [HideInInspector] public Animator anim;
    string currentState;

    [Header("Combat")]
    public GameObject projectilePrefab;
    public Transform muzzle;

    [Header("Weapon")]
    public float rateOfFire = 100;
    float fireRate;
    float fireTimer;

    const string IDLE = "idle";
    const string MOVE = "move";

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        fireRate = 1 / (rateOfFire / 60);
        fireTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        FindPlayer();
        Move();
        Aim();

        if (target && Vector2.Distance(transform.position, target.position) < detectionRadius / 2) Shoot();

        fireRate = 1 / (rateOfFire / 60);

        if (fireTimer < fireRate) fireTimer += Time.deltaTime;

        if (target && Vector2.Distance(transform.position, target.position) > safeDistance) 
            direction = target.position - transform.position;
        else 
            direction = Vector2.zero;

        if (health <= 0) Death();
    }

    void Move()
    {
        direction.Normalize();

        Vector3 targetVelocity = direction * speed;
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref m_velocity, m_MovementSmoothing);

        if (direction.magnitude > 0) ChangeAnimationState(MOVE);
        else ChangeAnimationState(IDLE);
    }

    void FindPlayer()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, detectionRadius, playerLayer);

        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].transform.CompareTag("Player"))
            {
                target = enemies[i].transform;
                break;
            }
        }

        if (enemies.Length <= 0)
            target = null;
    }

    void Aim()
    {
        if (target == null) return;

        Vector2 dir = target.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        muzzle.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward); ;
    }

    void Shoot()
    {
        if (fireTimer < fireRate) return;

        GameObject projectile = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);
        projectile.GetComponent<ProjectileController>().projectileSpeed = 16;
        projectile.GetComponent<ProjectileController>().isFromPlayer = false;

        //caneAnim.Play("fire");

        fireTimer = 0;
    }

    public void TakeDamage(float amount)
    {
        if (health >= amount) health -= amount;
        else health = 0;
    }

    void Death()
    {
        FindObjectOfType<WavesController>().EnemyKilled();
        gameObject.SetActive(false);
        //Destroy(gameObject);
    }

    #region Animation
    public void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;

        anim.Play(newState);

        currentState = newState;
    }
    #endregion
}
