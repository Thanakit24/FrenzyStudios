using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController player; //incase u dont know what static does, it enables other script to reference to call PlayerController.player.someFunction(); without needing to do serialized field stuffs

    [Header("Player")]
    public float _speed;
    public float _sensitivity, _jumpForce;
    [SerializeField] private Vector3 playerMovementInput;
    private Vector2 mouseMovementInput;
    private float xRotation;
    private bool isDashing, isJumping;
    [HideInInspector] public bool isKnocked = false;
    public float knockbackRecoveryTime = 0.2f;

    [Header("References")]
    [SerializeField] private Feet feet;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform playerCamera;
    private GameObject shuriken;
    public GameObject trail;

    [Header("Dash Config")]
    [SerializeField] private float dashForce;
    [SerializeField] private float dashCooldown;
    [SerializeField] private bool canDash = true;
    public bool threeDimensionalDashing = true;

    void Start()
    {
        player = this;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isDashing = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            isDashing = true;
            rb.useGravity = false;
        }

        trail.SetActive(isDashing);

        if (Input.GetKeyDown(KeyCode.E))
        {
            player.transform.position = GameObject.Find("Shuriken").transform.position;
        }

        if (Input.GetKeyDown(KeyCode.Space) && feet.isGrounded) isJumping = true;



        //if (Input.GetKeyDown(KeyCode.LeftShift)) StartCoroutine(Dash(transform.TransformDirection(playerMovementInput.normalized)));

    }

    void FixedUpdate()
    {
        playerMovementInput = Vector3.ClampMagnitude(new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")), 1f);
        mouseMovementInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        MovePlayerCamera();

        if (isDashing)
        {
            StartCoroutine(Dash(transform.TransformDirection(playerMovementInput.normalized)));
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

        if (isJumping)
        {
            Jump();
            isJumping = false;
        }

        //if (!feet.isGrounded)
        //rb.velocity = rb.velocity.y * 0.98f;
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

    void Jump()
    {
        if (Physics.CheckBox(transform.position - Vector3.up * 1.5f, Vector3.one * 0.5f))
        {
            rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
            rb.velocity = rb.velocity * 0.95f * Time.deltaTime;
        }
    }

    IEnumerator Dash(Vector3 dir)
    {
        if(canDash)
        {
            canDash = false;

            if (threeDimensionalDashing)
            {
                rb.AddForce(Camera.main.transform.forward * dashForce, ForceMode.Impulse);
            }
            else
            {
                rb.AddForce((dir) * dashForce, ForceMode.Impulse);
            }


            yield return new WaitForSeconds(dashCooldown);
            canDash = true;
            rb.useGravity = true;
            isDashing = false;
        }
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
