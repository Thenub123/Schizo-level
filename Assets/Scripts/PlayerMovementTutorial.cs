using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;

public class PlayerMovementTutorial : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [HideInInspector] public float walkSpeed;
    [HideInInspector] public float sprintSpeed;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;
    public Transform cam;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public Animator anim;

    public ParticleSystem shoot;
    public ParticleSystem burst;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;

        shoot.Pause();
        burst.Pause();
    }

    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);
        Vector3 mousePos = Input.mousePosition;

        // if(groundCheck.isGrounded) {
        //     grounded = true;
        // }
        // else {
        //     grounded = false;
        // }

        MyInput();
        SpeedControl();

        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        Debug.DrawRay(orientation.transform.position, cam.transform.TransformDirection(Vector3.forward), Color.green);

        if (Input.GetMouseButtonDown(0)){
            anim.SetTrigger("Shoot");
            ParticleSystem shoot_particle;
            ParticleSystem burst_particle;
            shoot_particle = Instantiate(shoot, shoot.transform.position, shoot.transform.rotation);
            shoot_particle.gameObject.SetActive(true);
            shoot_particle.Play();

            RaycastHit hit;

            if (Physics.Raycast(orientation.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, whatIsGround))
            {
                burst_particle = Instantiate(burst, hit.point, burst.transform.rotation);
                burst_particle.gameObject.SetActive(true);
                burst_particle.Play();

                if(hit.rigidbody) {
                    hit.rigidbody.AddForceAtPosition(1000 * cam.transform.TransformDirection(Vector3.forward), hit.point);
                }
            }

            
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on ground
        if(grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if(!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }
}