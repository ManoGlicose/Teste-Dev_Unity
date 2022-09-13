using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Components")] // Componentes necess�rios para o funcionamento do script
    Controls controls; // Novo Input System
    Rigidbody2D rb; 

    [Header("Status")] // // Informa��es sobre a vida do player
    public float health = 100;
    float damageMultiplier = 1; // Se o jogador estiver com pouca vida, ele receber� menos dano

    [Header("Movement")] // Par�metros de movimento
    public float speed = 4;
    Vector2 moveDirection;
    Vector3 m_velocity;
    float m_MovementSmoothing = .05f;

    [Header("Flip")] // Par�metros para rotacionar o jogador para olhar para o lado em que o mouse est�
    [HideInInspector] public bool isFacingRight;
    Vector2 mousePosition;

    [Header("Dash")] // Par�metros para a fun�ao de Dash
    public float dashPower = 24;
    bool canDash = true;
    [HideInInspector] public bool isDashing;
    float dashTime = .2f;
    float dashCooldown = .3f;

    [Header("Animation")] // Par�metros de anima��o
    [HideInInspector] public Animator anim;
    string currentState;

    [Header("UI")]
    public Image healthBar;
    public CanvasGroup gameOverScreen;

    [Header("Audio")] // Arquivos de Audio
    public AudioClip hurtClip;
    public AudioClip faintClip;
    public AudioClip flyByClip;
    AudioSource source;

    // Constantes que guardam o nome dos estados das anima��es 
    const string IDLE = "player_idle";
    const string MOVE = "player_move";

    private void Awake()
    {
        controls = new Controls(); // Cria o Input System

        controls.Movement.Move.performed += ctx => moveDirection = ctx.ReadValue<Vector2>(); // Define a vari�vel de posi��o com WASD
        controls.Movement.Move.canceled += _ => moveDirection = Vector2.zero; // Reseta a vari�vel se nada estiver sendo apertado

        controls.Movement.Dash.performed += ctx => PerformDash(moveDirection); // Executa o Dash

        controls.Combat.Aim.performed += ctx => mousePosition = ctx.ReadValue<Vector2>(); // Define a posi��o do mouse
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        Flip();

        transform.localScale = new Vector3(isFacingRight ? 1 : -1, 1, 1); // Muda a escala do player para olhar para o lado certo

        if (health <= 15) damageMultiplier = .75f; // Define o multiplicador de dano
        else damageMultiplier = 1;

        if (health <= 0) Death(); // Chama a fun��o de morte

        UIHandle();
    }

    private void FixedUpdate()
    {
        Move(moveDirection); // Controla o player
    }

    void Move(Vector2 direction) // Fun��o para controlar o player
    {
        Vector3 targetVelocity = direction * speed;
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref m_velocity, m_MovementSmoothing);

        if (direction.magnitude > 0) ChangeAnimationState(MOVE);
        else ChangeAnimationState(IDLE);
    }

    void Flip() // Fun��o para rotacionar o player
    {
        var playerScreenPoint = Camera.main.WorldToScreenPoint(transform.position);

        if (mousePosition.x < playerScreenPoint.x) isFacingRight = false;
        else isFacingRight = true;
    }

    void PerformDash(Vector2 direction) // Fun��o de Dash
    {
        if (canDash)
        {
            StartCoroutine(Dash(direction));
            //print("DASH " + direction.ToString());

            // Animation
            //if (direction > 0) ChangeAnimationState(DASH);
            //else if (direction < 0) ChangeAnimationState(DASH_BACKWARDS);

        }
    }

    IEnumerator Dash(Vector2 direction) // Controle de atraso do input de Dash
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;
        rb.velocity = direction * dashPower;
        source.PlayOneShot(flyByClip);

        yield return new WaitForSeconds(dashTime);

        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }

    public void TakeDamage(float amount) // Fun��o para aplicar dano
    {
        if (health >= amount)
        {
            health -= (amount * damageMultiplier);
            source.PlayOneShot(hurtClip, .5f); // Toca o som de dano
        }
        else
        {
            health = 0;
            GameObject.FindGameObjectWithTag("Respawn").GetComponent<AudioSource>().PlayOneShot(faintClip, .5f); // Toca o som de morte
            GameObject.FindGameObjectWithTag("Finish").GetComponent<AudioSource>().Stop(); // Para a m�sica de fundo
        }
    }

    void Death() // Fun��o de morte
    {
        gameOverScreen.alpha = 1;
        gameOverScreen.blocksRaycasts = true;

        gameObject.SetActive(false);
    }

    void UIHandle() // Fun��o que controla a UI
    {
        healthBar.fillAmount = health / 100;
    }

    #region Animation
    public void ChangeAnimationState(string newState) // Fun��o que executa as anima��es (muito melhor que o Mechanim em jogos 2D)
    {
        if (currentState == newState) return;

        anim.Play(newState);

        currentState = newState;
    }
    #endregion

    // Fun��es necess�rias para iniciar o novo Input System
    #region Enable/Disable Controls
    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }
    #endregion
}
