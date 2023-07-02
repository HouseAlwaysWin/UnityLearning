using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rigibody2D;
    private SpriteRenderer sr;

    private bool isDead;
    private Animator anim;
    private bool playerUnlocked;

    [Header("Knockback info")]
    [SerializeField]
    private Vector2 knockbackDir;
    private bool isKnocked;
    private bool canBeKnocked = true;

    [Header("Move Info")]
    [SerializeField]
    private float moveSpeed;
    [SerializeField]
    private float maxSpeed;
    private float defaultSpeed;
    [SerializeField]
    private float speedMultiplier;
    [Space]
    [SerializeField]
    private float milestoneIncreaser;
    private float defaultMilestoneIncreaser;
    private float speedMileStone;


    [Header("Jump Info")]
    [SerializeField]
    private float jumpForce;
    private bool canDoubleJump;
    [SerializeField]
    private float doubleJumpForce;


    private bool isGround;
    [Header("Collision Info")]
    [SerializeField]
    private float groundCheckDistance;
    [SerializeField]
    private LayerMask whatIsGround;
    [SerializeField]
    private Transform wallCheck;
    [SerializeField]
    private Vector2 wallCheckSize;
    private bool wallDetected;

    [Header("Slide Info")]
    [SerializeField]
    private float slideSpeed;
    [SerializeField]
    private float slideTime;
    [SerializeField]
    private float slideCoolDown;
    [SerializeField]
    private float ceilingCheckDistance;
    private float slideCoolDownCounter;
    private float slideTimeCounter;
    private bool isSliding;
    private bool ceillingDetected;
    [HideInInspector]
    public bool ledgeDetected;

    [Header("Ledge info")]
    [SerializeField] private Vector2 offset1;
    [SerializeField] private Vector2 offset2;

    private Vector2 climbBegunPosition;
    private Vector2 climbOverPosition;

    private bool canGarbLedge = true;
    private bool canClimb;




    void Awake()
    {
        groundCheckDistance = 1.4f;
    }

    // Start is called before the first frame update
    void Start()
    {
        rigibody2D = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        speedMileStone = milestoneIncreaser;
        defaultSpeed = moveSpeed;
        defaultMilestoneIncreaser = milestoneIncreaser;
    }

    // Update is called once per frame
    void Update()
    {

        CheckCollision();
        AnimatorControllers();

        slideTimeCounter -= Time.deltaTime;
        slideCoolDownCounter -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.K) && isGround)
        {
            Knockback();
        }

        if (Input.GetKeyDown(KeyCode.O) && isDead)
        {
            StartCoroutine(Die());
        }

        if (isKnocked || isDead)
        {
            return;
        }


        if (playerUnlocked)
        {
            SetupMovement();
        }

        if (isGround)
        {
            canDoubleJump = true;
        }

        SpeedController();

        CheckForLedge();
        CheckForSlideCancel();
        CheckInput();
    }

    public void Damage()
    {
        if (moveSpeed >= maxSpeed)
        {
            Knockback();
        }
        else
        {
            StartCoroutine(Die());
        }
    }

    private IEnumerator Die()
    {
        isDead = true;
        canBeKnocked = false;
        rigibody2D.velocity = knockbackDir;
        anim.SetBool("isDead", true);

        yield return new WaitForSeconds(.5f);
        rigibody2D.velocity = new Vector2(0, 0);
    }

    private IEnumerator Invincibility()
    {
        Color originalColor = sr.color;
        Color darkenColor = new Color(sr.color.r, sr.color.g, sr.color.b, .5f);

        canBeKnocked = false;

        sr.color = originalColor;
        yield return new WaitForSeconds(.1f);

        sr.color = darkenColor;
        yield return new WaitForSeconds(.1f);

        sr.color = originalColor;
        yield return new WaitForSeconds(.15f);

        sr.color = darkenColor;
        yield return new WaitForSeconds(.15f);
        sr.color = originalColor;
        yield return new WaitForSeconds(.25f);

        sr.color = darkenColor;
        yield return new WaitForSeconds(.25f);
        sr.color = originalColor;
        yield return new WaitForSeconds(.3f);

        sr.color = darkenColor;
        yield return new WaitForSeconds(.35f);

        sr.color = darkenColor;
        yield return new WaitForSeconds(.4f);

        sr.color = originalColor;
        canBeKnocked = true;
    }

    private void Knockback()
    {
        if (!canBeKnocked)
        {
            return;
        }
        StartCoroutine(Invincibility());
        isKnocked = true;
        rigibody2D.velocity = knockbackDir;
    }

    private void CancelKnockback() => isKnocked = false;

    private void SpeedReset()
    {
        moveSpeed = defaultSpeed;
        milestoneIncreaser = defaultMilestoneIncreaser;
    }

    private void SpeedController()
    {
        if (moveSpeed == maxSpeed)
        {
            return;
        }

        if (transform.position.x > speedMileStone)
        {
            speedMileStone = speedMileStone + milestoneIncreaser;

            moveSpeed *= speedMultiplier;
            milestoneIncreaser = milestoneIncreaser * speedMultiplier;

            if (moveSpeed > maxSpeed)
            {
                moveSpeed = maxSpeed;
            }
        }

    }


    private void LedgeClimbOver()
    {
        canClimb = false;
        rigibody2D.gravityScale = 5;
        transform.position = climbOverPosition;
        Invoke("AllowLedgeGrab", .1f);
    }

    private void AllowLedgeGrab()
    {
        canGarbLedge = true;
    }

    private void CheckForSlideCancel()
    {
        if (slideTimeCounter < 0 && !ceillingDetected)
        {
            isSliding = false;
        }
    }

    private void CheckForLedge()
    {
        if (ledgeDetected && canGarbLedge)
        {
            canGarbLedge = false;
            rigibody2D.gravityScale = 0;

            Vector2 ledgePosition = GetComponentInChildren<LedgeDetection>().transform.position;

            climbBegunPosition = ledgePosition + offset1;
            climbOverPosition = ledgePosition + offset2;

            canClimb = true;
        }

        if (canClimb)
        {
            transform.position = climbBegunPosition;
        }
    }

    private void SetupMovement()
    {


        if (isSliding && ceillingDetected)
        {
            rigibody2D.velocity = new Vector2(slideSpeed, rigibody2D.velocity.y);
        }
        else
        {
            if (wallDetected)
            {
                SpeedReset();
                return;
            }
            rigibody2D.velocity = new Vector2(moveSpeed, rigibody2D.velocity.y);
        }
    }

    void FixedUpdate()
    {

    }

    private void AnimatorControllers()
    {
        anim.SetFloat("yVelocity", rigibody2D.velocity.y);
        anim.SetFloat("xVelocity", rigibody2D.velocity.x);

        anim.SetBool("canDoubleJump", canDoubleJump);
        anim.SetBool("isGround", isGround);
        anim.SetBool("isSliding", isSliding);
        anim.SetBool("canClimb", canClimb);
        anim.SetBool("isKnocked", isKnocked);

        if (rigibody2D.velocity.y < -20)
        {
            anim.SetBool("canRoll", true);
        }
    }

    private void RollAnimFinished() => anim.SetBool("canRoll", false);

    private void CheckCollision()
    {
        isGround = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, whatIsGround);
        ceillingDetected = Physics2D.Raycast(transform.position, Vector2.up, ceilingCheckDistance, whatIsGround);
        wallDetected = Physics2D.BoxCast(
            origin: wallCheck.position,
            size: wallCheckSize,
            angle: 0,
            direction: Vector2.zero,
            distance: 0,
            layerMask: whatIsGround);
    }

    private void CheckInput()
    {
        if (Input.GetButtonDown("Jump"))
        {
            JumpButton();
        }

        if (Input.GetButtonDown("Fire1"))
        {
            playerUnlocked = true;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            SlidButton();
        }
    }

    private void SlidButton()
    {
        if (rigibody2D.velocity.x != 0 && slideCoolDownCounter < 0)
        {
            isSliding = true;
            slideTimeCounter = slideTime;
            slideCoolDownCounter = slideCoolDown;
        }
    }

    private void JumpButton()
    {
        if (isSliding)
        {
            return;
        }

        if (isGround)
        {
            // canDoubleJump = true;
            rigibody2D.velocity = new Vector2(rigibody2D.velocity.x, jumpForce);
        }
        else if (canDoubleJump)
        {
            canDoubleJump = false;
            rigibody2D.velocity = new Vector2(rigibody2D.velocity.x, doubleJumpForce);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y + ceilingCheckDistance));
        Gizmos.DrawCube(wallCheck.position, wallCheckSize);
    }
}
