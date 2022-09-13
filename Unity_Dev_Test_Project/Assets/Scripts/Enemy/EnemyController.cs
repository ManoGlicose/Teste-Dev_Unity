using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Components")] // Componentes necessários para o funcionamento do script
    Rigidbody2D rb;

    [Header("Status")] // Informações sobre a vida do inimigo
    public float health = 100;

    [Header("Movement")] // Parâmetros de movimento
    public float speed = 4;
    Vector3 m_velocity;
    float m_MovementSmoothing = .1f;
    Vector2 direction;

    [Header("Target")] // Alvo (jogador)
    public LayerMask playerLayer;
    public Transform target;

    [Header("AI Parameters")] // Parâmetros de inteligência 
    public float detectionRadius = 10;
    public float safeDistance = 3;

    [Header("Animation")] // Animaçoes
    [HideInInspector] public Animator anim;
    string currentState;

    [Header("Combat")] // Objetos usados para o disparo do projétil
    public GameObject projectilePrefab;
    public Transform muzzle;

    [Header("Weapon")] // Parâmetros de disparo 
    public float rateOfFire = 100;
    float fireRate;
    float fireTimer;

    [Header("Audio")] // Arquivos de audio
    public AudioClip fireClip;
    public AudioClip faintClip; 
    AudioSource source;

    // Constantes que guardam o nome dos estados das animações 
    const string IDLE = "idle";
    const string MOVE = "move";

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        source = GetComponent<AudioSource>();

        fireRate = 1 / (rateOfFire / 60);
        fireTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        FindPlayer();
        Move();
        Aim();

        // Só atira se tiver a 6 metros do jogador
        if (target && Vector2.Distance(transform.position, target.position) < 6) Shoot();

        // Calcula o intervalo entre um disparo e outro para o cálculo
        fireRate = 1 / (rateOfFire / 60);

        // Aumenta o contador do intervalo de disparo enquanto ele for menor que o intervalo
        if (fireTimer < fireRate) fireTimer += Time.deltaTime;

        // Para de seguir o jogador depois de uma certa distância
        if (target && Vector2.Distance(transform.position, target.position) > safeDistance) 
            direction = target.position - transform.position;
        else 
            direction = Vector2.zero;

        // Excecuta a morte do inimigo
        if (health <= 0) Death();
    }

    void Move() // Função de movimento
    {
        direction.Normalize();

        Vector3 targetVelocity = direction * speed;
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref m_velocity, m_MovementSmoothing);

        if (direction.magnitude > 0) ChangeAnimationState(MOVE);
        else ChangeAnimationState(IDLE);
    }

    void FindPlayer() // Detecta o jogador dentro do raio
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

    void Aim() // Mira em direção ao jogador
    {
        if (target == null) return;

        Vector2 dir = target.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        muzzle.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward); ;
    }

    void Shoot() // Função de disparo
    {
        if (fireTimer < fireRate) return;

        GameObject projectile = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);
        projectile.GetComponent<ProjectileController>().projectileSpeed = 16;
        projectile.GetComponent<ProjectileController>().isFromPlayer = false;

        //caneAnim.Play("fire");
        source.PlayOneShot(fireClip, .5f);

        fireTimer = 0;
    }

    public void TakeDamage(float amount) // Função de receber dano que é invocada pelo projétil que o acertou
    {
        if (health >= amount) health -= amount;
        else health = 0;
    }

    void Death() // Função de morte
    {
        FindObjectOfType<WavesController>().EnemyKilled();
        GameObject.FindGameObjectWithTag("Respawn").GetComponent<AudioSource>().PlayOneShot(faintClip, .75f);
        gameObject.SetActive(false);
        //Destroy(gameObject);
    }

    #region Animation
    public void ChangeAnimationState(string newState) // Função que executa as animações (muito melhor que o Mechanim em jogos 2D)
    {
        if (currentState == newState) return;

        anim.Play(newState);

        currentState = newState;
    }
    #endregion
}
