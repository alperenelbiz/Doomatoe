using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float slideSpeed;

    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Dash")]
    public float dashForce;
    public float dashDuration;
    public float dashCooldown;
    private bool dashing;
    private float dashEndTime;
    private float nextDashTime;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode dashKey = KeyCode.LeftAlt;
    public KeyCode slideKey = KeyCode.Z;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("Speed Boost")]
    public float boostedSpeed;
    public float boostDuration;


    [Header("References")]
    public Transform orientation;
    public Transform playerObject;
    Rigidbody rb;

    [Header("Sliding")]
    public float maxSlidingTime;
    public float slideForce;
    float timer;

    public float slideYscale;

    bool isSliding;


    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;



    public MovementState state;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        air,
        sliding
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        startYScale = transform.localScale.y;
    }

    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

        MyInput();
        SpeedControl();
        StateHandler();

        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        // handle dash input
        if (Input.GetKeyDown(dashKey) && !dashing && Time.time >= nextDashTime && grounded)
        {
            dashing = true;
            dashEndTime = Time.time + dashDuration;
            nextDashTime = Time.time + dashCooldown;
        }

        //handle sliding
        if (Input.GetKeyDown(slideKey) && (horizontalInput != 0 || verticalInput != 0) && OnSlope())
        {
            StartSliding();
        }

        if (Input.GetKeyUp(slideKey) && isSliding)
        {
            StopSliding();
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();

        if(isSliding)
        {
            SlidingMovement();
        }
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // start crouch
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        // stop crouch
        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }

        // handle dash input
        if (Input.GetKeyDown(dashKey) && !dashing && Time.time >= nextDashTime && grounded)
        {
            dashing = true;
            dashEndTime = Time.time + dashDuration;
            nextDashTime = Time.time + dashCooldown;
        }
    }

    private void StateHandler()
    {
        // Mode - Crouching
        if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            moveSpeed = crouchSpeed;
        }

        // Mode - Sprinting
        else if (grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }

        // Mode - Walking
        else if (grounded)
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }

        // Mode - sliding
        else if(isSliding)
        {
            state = MovementState.sliding;
            if(OnSlope() && rb.velocity.y < 0)
            {
                moveSpeed = slideSpeed;
            }
            else
            {
                moveSpeed = sprintSpeed;
            }

        }

        // Mode - Air
        else
        {
            state = MovementState.air;
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // apply dash force if dashing
        if (dashing && Time.time < dashEndTime)
        {
            rb.AddForce(moveDirection.normalized * dashForce, ForceMode.Impulse);
        }

        // turn off dash after duration
        if (Time.time >= dashEndTime)
        {
            dashing = false;
        }

        // reset dashing if dash cooldown is over
        if (Time.time >= nextDashTime)
        {
            dashing = false;
        }

        // on slope
        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        // on ground
        else if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        // turn gravity off while on slope
        rb.useGravity = !OnSlope();
    }

    private void SpeedControl()
    {
        // limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
                rb.velocity = rb.velocity.normalized * moveSpeed;
        }

        // limiting speed on ground or in air
        else if (!dashing)  // This condition prevents speed limiting during dash
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    private void Jump()
    {
        exitingSlope = true;

        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("SpeedBoost"))
        {
            StartCoroutine("ActivateSpeedBoost");
        }
    }

    IEnumerator ActivateSpeedBoost()
    {
        float origSpeed = walkSpeed;
        walkSpeed = boostedSpeed;
        
        yield return new WaitForSeconds(boostDuration);
        
        walkSpeed = origSpeed;
    }


    private void StartSliding()
    {
        isSliding = true;

        playerObject.localScale = new Vector3(playerObject.localScale.x, slideYscale, playerObject.localScale.z);
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        timer = maxSlidingTime;
    }

    private void SlidingMovement()
    {
        Vector3 directionOfInput = orientation.forward * verticalInput + orientation.forward * horizontalInput;

        if (!OnSlope() || rb.velocity.y > -0.1f)
        {
            rb.AddForce(directionOfInput.normalized * slideForce, ForceMode.Force);
            timer -= Time.deltaTime;
        }
        else
        {
            rb.AddForce(GetSlopeMoveDirection(directionOfInput) * slideForce, ForceMode.Force);
        }

        if (timer <= 0)
        {
            StopSliding();
        }
    }

    private void StopSliding()
    {
        isSliding = false;
        playerObject.localScale = new Vector3(playerObject.localScale.x, startYScale, playerObject.localScale.z);
    }

}

