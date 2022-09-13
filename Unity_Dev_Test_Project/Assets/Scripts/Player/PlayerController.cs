using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    Controls controls;
    Rigidbody2D rb;

    [Header("Status")]
    public float health = 100;
    float damageMultiplier = 1; // Se o jogador tiver pouca vida, ele receber� menos dano

    [Header("Movement")]
    public float speed = 4;
    Vector2 moveDirection;
    Vector3 m_velocity;
    float m_MovementSmoothing = .05f;

    [Header("Flip")]
    [HideInInspector] public bool isFacingRight;
    Vector2 mousePosition;

    [Header("Dash")]
    public float dashPower = 24;
    bool canDash = true;
    [HideInInspector] public bool isDashing;
    float dashTime = .2f;
    float dashCooldown = .3f;

    [Header("Animation")]
    [HideInInspector] public Animator anim;
    string currentState;

    [Header("UI")]
    public Image healthBar;
    public CanvasGroup gameOverScreen;

    [Header("Audio")]
    public AudioClip hurtClip;
    public AudioClip faintClip;
    public AudioClip flyByClip;
    AudioSource source;

    const string IDLE = "player_idle";
    const string MOVE = "player_move";

    private void Awake()
    {
        controls = new Controls();

        controls.Movement.Move.performed += ctx => moveDirection = ctx.ReadValue<Vector2>();
        controls.Movement.Move.canceled += _ => moveDirection = Vector2.zero;

        controls.Movement.Dash.performed += ctx => PerformDash(moveDirection);

        controls.Combat.Aim.performed += ctx => mousePosition = ctx.ReadValue<Vector2>();
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

        transform.localScale = new Vector3(isFacingRight ? 1 : -1, 1, 1);

        if (health <= 15) damageMultiplier = .75f;
        else damageMultiplier = 1;

        if (health <= 0) Death();

        UIHandle();
    }

    private void FixedUpdate()
    {
        Move(moveDirection);
    }

    void Move(Vector2 direction)
    {
        Vector3 targetVelocity = direction * speed;
        rb.velocity = Vector3.SmoothDamp(rb.velocity, targetVelocity, ref m_velocity, m_MovementSmoothing);

        if (direction.magnitude > 0) ChangeAnimationState(MOVE);
        else ChangeAnimationState(IDLE);
    }

    void Flip()
    {
        var playerScreenPoint = Camera.main.WorldToScreenPoint(transform.position);

        if (mousePosition.x < playerScreenPoint.x) isFacingRight = false;
        else isFacingRight = true;
    }

    void PerformDash(Vector2 direction)
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

    IEnumerator Dash(Vector2 direction)
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

    public void TakeDamage(float amount)
    {
        if (health >= amount)
        {
            health -= (amount * damageMultiplier);
            source.PlayOneShot(hurtClip, .5f);
        }
        else
        {
            health = 0;
            GameObject.FindGameObjectWithTag("Respawn").GetComponent<AudioSource>().PlayOneShot(faintClip, .5f);
            GameObject.FindGameObjectWithTag("Finish").GetComponent<AudioSource>().Stop();
        }
    }

    void Death()
    {
        gameOverScreen.alpha = 1;
        gameOverScreen.blocksRaycasts = true;

        gameObject.SetActive(false);
    }

    void UIHandle()
    {
        healthBar.fillAmount = health / 100;
    }

    #region Animation
    public void ChangeAnimationState(string newState)
    {
        if (currentState == newState) return;

        anim.Play(newState);

        currentState = newState;
    }
    #endregion

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
