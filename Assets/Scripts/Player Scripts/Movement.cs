using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements.Experimental;

public class Movement : MonoBehaviour
{
    private float playerHeight = 2f; 

    [Header("Movement")]
    [SerializeField] float moveSpeed = 5f;

    [Header("Jumping")]
    [SerializeField] float jumpForce = 5f;
    private int jumpCount = 0;
    private int maxJumps = 2;

    [Header("Dashing")]
    [SerializeField] float dashSpeed = 10f;
    [SerializeField] float dashDuration = 0.5f;
    [SerializeField] float dashCooldown = 2f;
    private bool isDashing = false;
    private bool canDash = true;

    [Header("Slope")]
    [SerializeField] float slopeForce;
    [SerializeField] float slopeForceRayLength;
    RaycastHit slopeHit;

    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;

    public Transform orientation;
    public Rigidbody rb;

    private void Start()
    {
        rb.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        HandleInput();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void HandleInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if (Input.GetButtonDown("Jump") && (IsGrounded() || jumpCount < maxJumps))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.E) && canDash)
        {
            StartCoroutine(Dash());
        }

    }
    private void Move()
    {
        if(!isDashing)
        {
            moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
            moveDirection.Normalize();

            // Controls the momentum caused by using AddForce
            SpeedControl(moveDirection);

            if ((verticalInput != 0 || horizontalInput != 0) && OnSlope())
            {
                rb.AddForce(Vector3.down * playerHeight / 2 * slopeForce * Time.fixedDeltaTime, ForceMode.Impulse);
            }
                

            /*if (OnSlope())
            {
                rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

                if (rb.velocity.y > 0)
                    rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }*/
        }

    }

    private void SpeedControl(Vector3 moveDirection)
    {
        Vector3 targetVelocity = new Vector3(moveDirection.x * moveSpeed, rb.velocity.y, moveDirection.z * moveSpeed);

        // Calculate the force required to reach the target velocity
        Vector3 force = (targetVelocity - rb.velocity) * rb.mass / Time.fixedDeltaTime;

        // Apply the force only to the horizontal components
        rb.AddForce(new Vector3(force.x, 0f, force.z));

        // Preserve vertical velocity to allow jumping
        rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, rb.velocity.z);
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        jumpCount++;
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 0.1f);
    }

    private bool OnSlope()
    {
        if(jumpCount > 0)
        {
            return false;
        }

        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2 * slopeForceRayLength))
        {
            if(slopeHit.normal != Vector3.up)
            {
                return true;
            }
        }
        return false;
    }

    /*private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }*/

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            jumpCount = 0;
        }
    }

    IEnumerator Dash()
    {
        isDashing = true;
        canDash = false;

        Vector3 dashDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        dashDirection.Normalize();

        // Character dashes forward as default if E is pressed when in idle state
        if((Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0) || (Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0 && !IsGrounded()))
        {
            dashDirection = orientation.forward;
            dashDirection.Normalize();
        }

        float dashForce = dashSpeed / dashDuration;

        rb.velocity = dashDirection * dashForce;

        float reducedDashDuration = dashDuration / 2f;

        yield return new WaitForSeconds(reducedDashDuration);

        if (!isDashing)
        {
            rb.velocity = Vector3.zero;
        }

        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
        isDashing = false;
    }
}
