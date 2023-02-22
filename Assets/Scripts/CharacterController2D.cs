using UnityEngine;
using UnityEngine.Events;

// From: https://github.com/Brackeys/2D-Character-Controller

public enum CharacterState { Idle, Moving, Shooting, Grappling, Crouching, Jumping, Landing }

public class CharacterController2D : MonoBehaviour
{
    [SerializeField] private float m_JumpForce = 400f;                          // Amount of force added when the player jumps.
    [Range(0, 1)] [SerializeField] private float m_CrouchSpeed = .36f;          // Amount of maxSpeed applied to crouching movement. 1 = 100%
    [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .05f;  // How much to smooth out the movement
    [SerializeField] private bool m_AirControl = false;                         // Whether or not a player can steer while jumping;
    [SerializeField] private LayerMask m_WhatIsGround;                          // A mask determining what is ground to the character
    [SerializeField] private Transform m_GroundCheck;                           // A position marking where to check if the player is grounded.
    [SerializeField] private Transform m_CeilingCheck;                          // A position marking where to check for ceilings
    [SerializeField] private Collider2D m_CrouchDisableCollider;                // A collider that will be disabled when crouching

    const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;            // Whether or not the player is grounded.
    const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
    private Rigidbody2D m_Rigidbody2D;
    private bool m_FacingRight = true;  // For determining which way the player is currently facing.
    private Vector3 m_Velocity = Vector3.zero;

    private PhysicsMaterial2D _groundMaterial, _airMaterial;

    private float m_lastJump;

    public CharacterState State { get; private set; }

    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;
    public UnityEvent OnJumpEvent;
    public UnityEvent OnIdleEvent;
    public UnityEvent OnMoveEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    public BoolEvent OnCrouchEvent;
    private bool m_wasCrouching = false;
    private bool m_wasMoving = false;
    private bool m_Moving;

    private void Awake()
    {
        m_Rigidbody2D = GetComponent<Rigidbody2D>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        if (OnCrouchEvent == null)
            OnCrouchEvent = new BoolEvent();

        _groundMaterial = m_Rigidbody2D.sharedMaterial;
        _airMaterial = new PhysicsMaterial2D("AirMaterial");
        _airMaterial.bounciness = _groundMaterial.bounciness;
        _airMaterial.friction = 0;
    }

    private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

    public void HandleFixedUpdate()
    {
        bool wasGrounded = m_Grounded;
        m_Grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
                if (stopwatch.ElapsedMilliseconds > (long)60)
                    OnLandEvent.Invoke();
                stopwatch.Reset();
                break;
            }
        }
        if (!m_Grounded)
            stopwatch.Start();

        if (m_Grounded != wasGrounded)
        {
            if (!wasGrounded)
            {
                // 1 frame jumps can mess stuff up
                m_lastJump = Time.fixedTime;
            }
            m_Rigidbody2D.sharedMaterial = m_Grounded ? _groundMaterial : _airMaterial;
        }

        if (!m_Grounded)
        {
            m_Rigidbody2D.AddForce(new Vector2(0, GlobalSettings.i.Gravity));
        }
    }


    public void Move(float move, bool crouch, bool jump)
    {
        m_wasMoving = m_Moving;
        m_Moving = false;

        // If crouching, check to see if the character can stand up
        if (!crouch)
        {
            // If the character has a ceiling preventing them from standing up, keep them crouching
            if (Physics2D.OverlapCircle(m_CeilingCheck.position, k_CeilingRadius, m_WhatIsGround))
            {
                crouch = true;
            }
        }

        //only control the player if grounded or airControl is turned on
        if (m_Grounded || m_AirControl)
        {

            // If crouching
            if (crouch)
            {
                if (!m_wasCrouching)
                {
                    m_wasCrouching = true;
                    OnCrouchEvent.Invoke(true);
                }

                // Reduce the speed by the crouchSpeed multiplier
                move *= m_CrouchSpeed;

                // Disable one of the colliders when crouching
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = false;
            }
            else
            {
                // Enable the collider when not crouching
                if (m_CrouchDisableCollider != null)
                    m_CrouchDisableCollider.enabled = true;

                if (m_wasCrouching)
                {
                    m_wasCrouching = false;
                    OnCrouchEvent.Invoke(false);
                }
            }

            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(move * 10f, m_Rigidbody2D.velocity.y);


            var smoothing = m_MovementSmoothing;
            var xAbs = Mathf.Abs(m_Rigidbody2D.velocity.x);
            var xSign = Mathf.Sign(m_Rigidbody2D.velocity.x);
            if (m_AirControl && (xAbs > 10f || (xSign == Mathf.Sign(move))))
            {
                smoothing *= 100;
            }

            // And then smoothing it out and applying it to the character
            m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, smoothing);

            // If the input is moving the player right and the player is facing left...
            if (move > 0 && !m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
            // Otherwise if the input is moving the player left and the player is facing right...
            else if (move < 0 && m_FacingRight)
            {
                // ... flip the player.
                Flip();
            }
        }

        // If the player should jump...
        if (m_Grounded && jump && m_lastJump + 0.1f < Time.fixedTime)
        {
            m_lastJump = Time.fixedTime;
            // Add a vertical force to the player.
            m_Grounded = false;
            m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
            OnJumpEvent.Invoke();
        }

        if (Mathf.Abs(move) > 0)
        {
            m_Moving = true;
            if (m_Moving != m_wasMoving && !crouch && !jump)
                OnMoveEvent.Invoke();
        }

        if (Mathf.Abs(move) == 0 && m_Moving != m_wasMoving && !crouch && !jump)
        {
            OnIdleEvent.Invoke();
        }
    }


    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
}
