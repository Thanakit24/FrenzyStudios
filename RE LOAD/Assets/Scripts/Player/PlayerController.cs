using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    [Header("Player")]
    public float _speed;
    public float _sensitivity;
    [SerializeField] private Vector3 playerMovementInput;
    private Vector2 mouseMovementInput;
    private float xRotation;
    private bool isDashing, isJumping;
    [HideInInspector] public bool isKnocked = false;
    public float knockbackRecoveryTime = 0.2f;
    private float horizontalMovement;

    [Header("Melee Attack Config")]
    public bool isMeleeing = false;
    public float meleeCooldown = 0.5f;
    public Collider attackArea;
    public int meleeDamage = 1;

    [Header("Jump Config")]
    public float jumpSpeed;
    public bool holdSpaceToJumpHigher = false;
    private float jumpTimeCounter;
    public float maxJumpTime;

    [Header("References")]
    [SerializeField] private Feet feet;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform playerCamera;
    public FumaController shuriken;
    public GameObject trail;

    [Header("Dash Config")]
    [SerializeField] private float dashForce;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashCooldown;
    private float dashCooldownTimer;
    [SerializeField] private bool canDash = true;
    public bool threeDimensionalDashing = true;

    void Start()
    {
        instance = this;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isDashing = false;

        dashCooldownTimer = dashCooldown;
    }

    private void Update()
    {
        //jumpKeyPressing = JumpKeyPressingChecker();

        if (Input.GetKeyDown(KeyCode.Space) && !isDashing && feet.isGrounded)
        {
            isJumping = true;
            rb.velocity = Vector3.up * jumpSpeed;
            jumpTimeCounter = maxJumpTime;
        }

        if (dashCooldownTimer > dashCooldown)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                isDashing = true;
                rb.useGravity = false;
                dashCooldownTimer = 0;
            }
        }
        else
        {
            dashCooldownTimer += Time.deltaTime;
        }

        

        trail.SetActive(isDashing);

        if (Input.GetKeyDown(KeyCode.E) && shuriken.state != FumaState.InHands)
        {
            instance.transform.position = shuriken.transform.position;
            shuriken.Returned();
        }

        if ((Input.GetKeyUp(KeyCode.Space) && isJumping))
        {
            isJumping = false;
        }

        if (Input.GetKeyDown(KeyCode.Comma))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //temporary for testing purposes
        }

        if (Input.GetKeyDown(KeyCode.V) && shuriken.state == FumaState.InHands && !isMeleeing && !isDashing)
        {
            Melee();
        }
    }

    void FixedUpdate()
    {
        

        playerMovementInput = Vector3.ClampMagnitude(new Vector3(horizontalMovement, 0, Input.GetAxisRaw("Vertical")), 1f);

        if (Input.GetAxisRaw("Horizontal") == 0)
        {
            horizontalMovement = 0;
        }
        else
        {
            horizontalMovement = Input.GetAxis("Horizontal");
        }

        mouseMovementInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        MovePlayerCamera();

        if (isDashing)
        {
            StartCoroutine(Dash(transform.TransformDirection(playerMovementInput.normalized)));

            if (Physics.OverlapSphere(transform.position + transform.forward,0.8f).Length > 2)
            {
                rb.velocity = Vector3.zero;
                rb.useGravity = true;
                isDashing = false;
            }
        }
        else
        {
            if (isKnocked)
            {
                StartCoroutine(Knocked(2));
            }
            else
                MovePlayer();

        }

        if (Input.GetKey(KeyCode.Space) && isJumping && holdSpaceToJumpHigher && !isDashing)
        {
            if (jumpTimeCounter > 0)
            {
                jumpTimeCounter -= Time.deltaTime;

                rb.velocity = new Vector3(rb.velocity.x, jumpSpeed - jumpTimeCounter * 2, rb.velocity.z );
            }
            else
            {
                isJumping = false;
            }
        }
    }

    

    private void MovePlayer()
    {
        Vector3 moveVector = transform.TransformDirection(playerMovementInput * _speed);
        rb.velocity = new Vector3(moveVector.x, rb.velocity.y, moveVector.z);
    }

    private void MovePlayerCamera()
    {
        xRotation -= mouseMovementInput.y * _sensitivity;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.Rotate(0f, mouseMovementInput.x * _sensitivity, 0f);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
    }

    IEnumerator Dash(Vector3 dir)
    {
        if(canDash)
        {
            rb.velocity = Vector3.zero;
            canDash = false;

            if (threeDimensionalDashing)
            {
                //rb.AddForce(Camera.main.transform.forward * dashForce, ForceMode.Impulse);
                rb.velocity = Camera.main.transform.forward * dashForce;
            }
            else
            {
                rb.AddForce((dir) * dashForce, ForceMode.Impulse);
            }


            yield return new WaitForSeconds(dashDuration);
            canDash = true; 
            rb.useGravity = true;
            isDashing = false;

            rb.velocity = Vector3.zero;
        }
    }

    public void Melee()
    {
        isMeleeing = true;

        foreach (Collider collider in Physics.OverlapSphere(transform.position + Vector3.forward * 0.91f, 0.78f))
        {
            if (collider.CompareTag("Enemy"))
            {
                print("enemy Found");

                if (collider.GetComponent<Health>() != null)
                {
                    collider.GetComponent<Health>().TakeDamage(meleeDamage);
                    print("something hit");
                }
            }
        }

        //put delay for the animation;
        isMeleeing = false;
    }

    IEnumerator Knocked(float knockbackForce)
    {
            //Debug.Log(dir);
            //rb.AddForce((dir + Vector3.up) * dashForce, ForceMode.Impulse);
            //rb.AddForce(Vector3.back * knockbackForce, ForceMode.Impulse);
            yield return new WaitForSeconds(knockbackRecoveryTime);
            isKnocked = false;
    }
}
