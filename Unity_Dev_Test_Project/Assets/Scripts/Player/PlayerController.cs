using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    Controls controls;
    Rigidbody2D rb;

    [Header("Movement")]
    public float speed = 4;
    Vector2 moveDirection;
    Vector3 m_velocity;
    float m_MovementSmoothing = .05f;

    [Header("Dash")]
    public float dashPower = 24;
    bool canDash = true;
    [HideInInspector] public bool isDashing;
    float dashTime = .2f;
    float dashCooldown = .3f;

    [Header("Animation")]
    [HideInInspector] public Animator anim;
    string currentState;

    const string IDLE = "player_idle";
    const string MOVE = "player_move";

    private void Awake()
    {
        controls = new Controls();

        controls.Movement.Move.performed += ctx => moveDirection = ctx.ReadValue<Vector2>();
        controls.Movement.Move.canceled += _ => moveDirection = Vector2.zero;

        controls.Movement.Dash.performed += ctx => PerformDash(moveDirection);
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
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

        yield return new WaitForSeconds(dashTime);

        rb.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
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
