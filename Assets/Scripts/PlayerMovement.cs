using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using System.Linq;

public class PlayerMovement : MonoBehaviour
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
    public LayerMask whatIsEnemy;
    public bool grounded;

    public Transform orientation;
    public Transform cam;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public int health;

    public Animator[] gun_anim;
    public Animator arm;
    public float[] cooldowns;

    bool canShoot = true;
    public bool canSlap = true;

    public int current_weapon;

    public ParticleSystem shoot;
    public ParticleSystem burst;

    public Animator screenShake;

    public TMP_Text healthText;

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

        healthText.text = "Health: " + health.ToString();

        MyInput();
        SpeedControl();

        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        Debug.DrawRay(orientation.transform.position, cam.transform.TransformDirection(Vector3.forward), Color.green);

        if(Input.GetAxis("Mouse ScrollWheel") > 0) {
            if(gun_anim.Length - 1 > current_weapon) {
                current_weapon += 1;
            } else {
                current_weapon = 0;
            }
            
        }

        if(Input.GetAxis("Mouse ScrollWheel") < 0) {
            if(current_weapon == 0) {
                current_weapon = gun_anim.Length - 1;
            } else {
                current_weapon -= 1;
            }
        }

        foreach (Animator anim in gun_anim) {
            if(canSlap){
                if (gun_anim[current_weapon] != anim) {
                    anim.SetBool("Disabled", true);
                }
                else{
                    anim.SetBool("Disabled", false);
                }
            } else {
                anim.SetBool("Disabled", true);
            }

        }

        if (Input.GetMouseButtonDown(0) && canShoot == true){
            canShoot = false;
            IEnumerator cooldown = Cooldown();
            StartCoroutine(cooldown);

            gun_anim[current_weapon].SetTrigger("Shoot");
            screenShake.SetTrigger("Shake");
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
                
                if(current_weapon == 1) {
                    burst_particle = Instantiate(burst, hit.point, burst.transform.rotation);
                    burst_particle.gameObject.SetActive(true);
                    burst_particle.Play();
                }

                if(hit.rigidbody) {
                    hit.rigidbody.AddForceAtPosition(1000 * cam.transform.TransformDirection(Vector3.forward), hit.point);
                }
                if(current_weapon == 1) {
                    rb.AddRelativeForce(-cam.transform.TransformDirection(Vector3.forward) * 1000);
                }
            }

            if (Physics.Raycast(orientation.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, whatIsEnemy))
            {
                burst_particle = Instantiate(burst, hit.point, burst.transform.rotation);
                burst_particle.gameObject.SetActive(true);
                burst_particle.Play();

                if(hit.collider.GetComponent<Enemy>()) {
                    hit.collider.GetComponent<Enemy>().Hit();
                    hit.collider.GetComponent<Enemy>().health -= 10;
                }
            }
        }
        

        if(Input.GetKeyDown(KeyCode.F) && canSlap) {
            IEnumerator cooldown = Slap();
            canSlap = false;
            canShoot = false;
            gun_anim[current_weapon].SetBool("Disabled", true);
            arm.SetTrigger("Hit");
            screenShake.SetTrigger("Shake");
            StartCoroutine(cooldown);

            RaycastHit hit;

            if (Physics.Raycast(orientation.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit, 2, whatIsGround))
            {
                if(hit.rigidbody) {
                    hit.rigidbody.AddForceAtPosition(1000 * cam.transform.TransformDirection(Vector3.forward), hit.point);
                }
            }

            if (Physics.Raycast(orientation.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit, 2, whatIsEnemy))
            {
                ParticleSystem burst_particle;
                burst_particle = Instantiate(burst, hit.point, burst.transform.rotation);
                burst_particle.gameObject.SetActive(true);
                burst_particle.Play();
                if(hit.collider.GetComponent<Enemy>()) {
                    hit.collider.GetComponent<Enemy>().Hit();
                    hit.collider.GetComponent<Enemy>().health -= 10;
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

    IEnumerator Cooldown() {
        yield return new WaitForSeconds(cooldowns[current_weapon]);
        canShoot = true;
    }

    IEnumerator Slap() {
        yield return new WaitForSeconds(0.35f);
        canSlap = true;
        canShoot = true;
        gun_anim[current_weapon].SetBool("Disabled", false);
    }
    
}