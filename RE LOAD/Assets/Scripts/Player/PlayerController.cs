using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController player; //incase u dont know what static does, it enables other script to reference to call PlayerController.player.someFunction(); without needing to do serialized field stuffs

    [Header("Player")]
    public float _speed, _sensitivity, _jumpForce;
    private Vector3 playerMovementInput;
    private Vector2 mouseMovementInput;
    private float xRotation;

    [Header("References")]
    [SerializeField] private Feet feet;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform playerCamera;

    [Header("Dash Config")]
    [SerializeField] private float dashForce;
    [SerializeField] private float dashCooldown;
    private bool canDash = true;


    void Start()
    {
        player = this;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        playerMovementInput = Vector3.ClampMagnitude(new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")), 1f);
        mouseMovementInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        MovePlayer(); 
        MovePlayerCamera();


        if (!feet.isGrounded)
        {
            rb.velocity = rb.velocity * 0.98f;
            Debug.Log(rb.velocity);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            StartCoroutine(Dash());
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

    void Jump()
    {
        if (feet.isGrounded)
            if (Physics.CheckBox(transform.position - Vector3.up * 1.5f, Vector3.one * 0.5f))
            {
                rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
                rb.velocity = rb.velocity * 0.95f * Time.deltaTime;
            }
    }

    IEnumerator Dash()
    {
        if(canDash)
        {
            canDash = false;
            rb.AddForce(this.gameObject.transform.forward * dashForce, ForceMode.Impulse);
            yield return new WaitForSeconds(dashCooldown);
            canDash = true;
        }
    }
}
