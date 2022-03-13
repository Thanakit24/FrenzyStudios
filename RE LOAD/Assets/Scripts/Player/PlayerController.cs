using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MoreMountains.Feedbacks;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    //public enum camState { _default, aiming, recieving, landing};
    //public camState currentCameraState = camState._default;

    [HideInInspector] public Vector3 rawPlayerMovementInput;
    [HideInInspector] public Vector3 PlayerMovementInput;
    [HideInInspector] public Vector3 smartPlayerMovementInput;
    private Vector2 mouseMovementInput;
    private float xRotation;
    private bool isDashing, isJumping;

    [Header("Movements")]
    public float actualSpeed;
    public float acceleration;
    public float initialVelocity;
    public float airControl;
    public float groundMaxVelocity;
    public float airMaxVelocity;
    [Range(0f, 1f)]
    public float movementThreshold = 0.7f;
    public Vector3 slopeMoveVector;

    [Header("Mouse")]
    public float _sensitivity;
    public float cameraJumpFeedbackCurrent;
    [Range(0.5f, 9f)]
    public float cameraJumpFeedbackThreshold;
    public float cameraJumpFeedbackDownMultiplier;
    public float cameraJumpFeedbackUpMultiplier;
    public float cameraJumpRecoverySpeed;
    public float cameraImpactTiltMax;
    [Space(7)]
    public float camYPosModifierMultiplier;
    public float camYPosRecoveryMultiplier;
    public float MaxCamYPos;
    public bool recoveringCamYPos;
    public bool recoveringCamTilt;
    public bool impactingCamYPos;
    private float recoveryCamYPosOffset;
    private float initialCamYPos;
    private float targetYPos;
    [Range(0f, 1f)]
    public float maxCamYPosImpactFromVelocity,maxCamTiltImpactFromVelocity;


    [Header("Player Status")]
    public bool isKnocked = false;
    public float knockbackRecoveryTime = 0.2f;
    private float horizontalMovement;
    public bool isTeleporting;
    public bool isGrounded;
    public bool isOnSlope;
    Vector3 colBottom;
    Vector3 colCurve;
    RaycastHit floorInfo;

    [Header("Melee Attack Config")]
    public bool isMeleeing = false;
    public float meleeCooldown = 0.5f;
    private float meleeCounter = 0f;
    public GameObject playerMeleeController;

    [Header("Jump Config")]
    private bool isInJumpPhase;
    public float jumpSpeed;
    public bool holdSpaceToJumpHigher = false;
    private float jumpTimeCounter;
    public float maxJumpTime;

    [Header("Dash Config")]
    [SerializeField] private float dashForce;
    [SerializeField] private float dashDuration;
    [SerializeField] private float dashCooldown;
    private float dashCooldownTimer;
    [SerializeField] private bool canDash = true;
    public bool threeDimensionalDashing = true;


    [Header("References")]
    public float halfOfHeight;
    [SerializeField] public Feet feet;
    [HideInInspector] public Rigidbody rb;
    [SerializeField] private Transform playerCamera;
    public FumaController shuriken;
    public GameObject trail;
    public Animator playerAnimator;
    public Transform camHolder;
    CapsuleCollider capCollider;

    [Header("Feedbacks")]
    public MMFeedbacks jumpImpact;
    public MMFeedbacks teleportWithSlowmoFB;
    public MMFeedbacks meleeSFX;
    public MMFeedbacks dashRecovery;

    void Start()
    {
        instance = this;
        capCollider = this.gameObject.GetComponent<CapsuleCollider>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isDashing = false;

        EventManager.Teleport += TeleportTo;

        dashCooldownTimer = dashCooldown;
        isTeleporting = false;

        if (airControl > 1) airControl = 1;
        if (airControl <= 0) airControl = 0.1f;

        impactingCamYPos = false;
        recoveringCamYPos = false;
        camHolder.localPosition = Vector3.zero;
        cameraJumpFeedbackCurrent = 0;

        Vector3 bottom = capCollider.bounds.center - (Vector3.up * capCollider.bounds.extents.y);
        Vector3 curve = bottom + (Vector3.up * capCollider.radius);
    }

    private void Update()
    {
        if (GameCanvasController.instance.currentState.Equals(GameState.paused))
        {
            PlayerMovementInput = Vector3.zero;
            rawPlayerMovementInput = Vector3.zero;
            smartPlayerMovementInput = Vector3.zero;
            rb.isKinematic = true;

            return;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;

            rb.isKinematic = false;
        }

        if (GameCanvasController.instance.currentState.Equals(GameState.dies)) return;

        #region Movement System

        rawPlayerMovementInput = Vector3.ClampMagnitude(new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")), 1f);
        PlayerMovementInput = Vector3.ClampMagnitude(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")), 1f);

        float tempMovX = Input.GetAxis("Horizontal");
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            tempMovX = Input.GetAxisRaw("Horizontal");
        }
        else if (Mathf.Abs(Input.GetAxis("Horizontal")) < movementThreshold)
        {
            tempMovX = 0;
        }

        float tempMovY = Input.GetAxis("Vertical");
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            tempMovY = Input.GetAxisRaw("Vertical");
        }
        else if (Mathf.Abs(Input.GetAxis("Vertical")) < movementThreshold)
        {
            tempMovY = 0;
        }
        smartPlayerMovementInput = Vector3.ClampMagnitude(new Vector3(tempMovX, 0, tempMovY), 1f);

        #endregion

        #region Jump + CamSystem

        if (Input.GetKeyDown(KeyCode.Space) && !isDashing && feet.isGrounded)
        {
            isJumping = true;
            isInJumpPhase = true;
            impactingCamYPos = false;
            recoveringCamYPos = false;
            camHolder.localPosition = Vector3.zero;
            cameraJumpFeedbackCurrent = 0;

            rb.velocity = Vector3.up * jumpSpeed;
            jumpTimeCounter = maxJumpTime;
        }

        if (isInJumpPhase)
        {
            if (rb.velocity.y < cameraJumpFeedbackThreshold)
            {
                //cameraJumpFeedbackCurrent = cameraJumpFeedbackInitial;

                if (feet.isGrounded)
                {
                    impactingCamYPos = true;
                    if (cameraJumpFeedbackCurrent <= 0.6f)
                    {
                        cameraJumpFeedbackCurrent -= 0.012f * Time.deltaTime;
                    }
                    else if (cameraJumpFeedbackCurrent <= 0.5f)
                    {
                        cameraJumpFeedbackCurrent = 0;
                        isInJumpPhase = false;
                    }
                }

                if (rb.velocity.y > 0)
                    cameraJumpFeedbackCurrent -= Time.deltaTime * cameraJumpFeedbackUpMultiplier;
                else if (rb.velocity.y < 0)
                {
                    cameraJumpFeedbackCurrent += Time.deltaTime * cameraJumpFeedbackDownMultiplier;
                    if (rb.velocity.y < -3f && camHolder.localPosition.y > MaxCamYPos * 1.5f)
                    {
                        camHolder.localPosition += Vector3.down * 3 * Time.deltaTime;
                    }
                }
            }
        }

        if (impactingCamYPos) CamImpactModify();
        if (recoveringCamYPos) CamYPosRecover();
        if (recoveringCamTilt) CamTiltRecover();

        if ((Input.GetKeyUp(KeyCode.Space) && isJumping))
        {
            isJumping = false;
        }
        #endregion

        #region Dash

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

        #endregion

        #region Teleport
        if (Input.GetKeyDown(KeyCode.E) && shuriken.state != FumaState.InHands)
        {
            isTeleporting = true;
            shuriken.state = FumaState.Stuck;
            shuriken.rb.velocity = Vector3.zero;
            teleportWithSlowmoFB.PlayFeedbacks();

            //Time.timeScale = 0.1f;
            //TeleportTo(shuriken.teleportLocation);
        }

        #endregion

        #region Melee
        if (meleeCounter > 0)
        {
            meleeCounter -= Time.deltaTime;
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0) && shuriken.state == FumaState.InHands && !isMeleeing && !isDashing)
        {
            meleeSFX.PlayFeedbacks();
            isMeleeing = true;
            playerAnimator.SetBool("isMeleeing", isMeleeing);
        }

        if (!isDashing)
            Camera.main.transform.localPosition = Vector3.zero + Vector3.up * 0.75f;

        playerMeleeController.SetActive(isMeleeing);

        #endregion

        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); //temporary for testing purposes
        }
    }

    void FixedUpdate()
    {
        if (GameCanvasController.instance.currentState.Equals(GameState.paused)) return;
        if (GameCanvasController.instance.currentState.Equals(GameState.dies)) return;


        mouseMovementInput = new Vector2(Input.GetAxis("Mouse X") * Time.deltaTime * 50, Input.GetAxis("Mouse Y") * Time.deltaTime * 50);
        MovePlayerCamera();
        if (!isDashing & !isKnocked) MovePlayerVelocity();

        if (isDashing)
        {
            StartCoroutine(Dash(transform.TransformDirection(PlayerMovementInput.normalized)));

            if (Physics.OverlapSphere(transform.position + transform.forward, 0.8f).Length > 2)
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
                //StartCoroutine(Knocked(2));
                isKnocked = false;
            }
        }

        if (Input.GetKey(KeyCode.Space) && isJumping && holdSpaceToJumpHigher && !isDashing)
        {
            if (jumpTimeCounter > 0)
            {
                jumpTimeCounter -= Time.deltaTime;

                rb.velocity = new Vector3(rb.velocity.x, jumpSpeed - jumpTimeCounter * 2, rb.velocity.z);
                //rb.AddForce(transform.TransformDirection(fakePlayerMovementInput) * 10);
            }
            else
            {
                isJumping = false;
            }
        }
    }


    public void OnGrounded()
    {
        impactingCamYPos = true;
        initialCamYPos = camHolder.localPosition.y;
        targetYPos = rb.velocity.y;
    }

    #region Camera

    private void CamYPosLimit()
    {
        float x = rb.velocity.y * maxCamYPosImpactFromVelocity;
        if (x > .5f) x = .5f;

        if (camHolder.localPosition.y < MaxCamYPos - x) camHolder.localPosition = new Vector3(camHolder.localPosition.x, MaxCamYPos-x, camHolder.localPosition.z);
    }

    private void MovePlayerCamera()
    {
        float tempSens = _sensitivity;

        //if (teleportWithSlowmoFB.IsPlaying) tempSens = _sensitivity / 2;

        xRotation -= mouseMovementInput.y * tempSens;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.Rotate(0f, mouseMovementInput.x * tempSens, 0f);
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation + cameraJumpFeedbackCurrent, 0, 0);
    }

    private void CamImpactModify()
    {
        if (recoveringCamTilt && recoveringCamYPos)
        {
            impactingCamYPos = false;
            return;
        }

        //float targetYPos = - cameraJumpFeedbackCurrent;
        float currentYPos = camHolder.localPosition.y;
        isInJumpPhase = false;

        if (cameraJumpFeedbackCurrent > (1 - maxCamTiltImpactFromVelocity) * cameraImpactTiltMax + rb.velocity.y * maxCamTiltImpactFromVelocity)
        {
            recoveringCamYPos = true;
            recoveringCamTilt = true;
            return;
        }
        if (currentYPos < (1-maxCamYPosImpactFromVelocity) * MaxCamYPos + rb.velocity.y - rb.velocity.y * maxCamYPosImpactFromVelocity)
        {
            recoveringCamTilt = true;
            recoveringCamYPos = true;
            return;
        }


        cameraJumpFeedbackCurrent += cameraJumpFeedbackDownMultiplier * Time.deltaTime * 2;
        camHolder.localPosition += Vector3.down * camYPosModifierMultiplier * Time.deltaTime;
    }

    private void CamYPosRecover()
    {
        if (camHolder.localPosition == Vector3.zero)
        {
            recoveringCamYPos = false;
        }

        if (camHolder.localPosition.y < 0)
        {
            camHolder.localPosition += Vector3.up * Time.deltaTime * camYPosRecoveryMultiplier;
        }

        //Reset
        if (Mathf.Abs(camHolder.localPosition.y) < 0.01f)
        {
            camHolder.localPosition = Vector3.zero;
        }
    }

    private void CamTiltRecover()
    {
        if (cameraJumpFeedbackCurrent == 0)
        {
            recoveringCamTilt = false;
        }

        if (cameraJumpFeedbackCurrent > 0)
            cameraJumpFeedbackCurrent -= cameraJumpRecoverySpeed * Time.deltaTime;
        else
            cameraJumpFeedbackCurrent += cameraJumpRecoverySpeed * Time.deltaTime;

        if (Mathf.Abs(cameraJumpFeedbackCurrent) < 0.2f)
        {
            cameraJumpFeedbackCurrent = 0;
        }
    }

    #endregion

    #region Movements Methods
    private void MovePlayerVelocity()
    {

        //Acceleration
        if (PlayerMovementInput.magnitude == 0)
        {
            actualSpeed = initialVelocity;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            if (actualSpeed < groundMaxVelocity)
                actualSpeed += acceleration;
        }

        //maxVelocityCheck
        if (!feet.isGrounded)
        {
            if (vectorPasser(rb.velocity, 0).magnitude > airMaxVelocity)
            {
                //Vector3 tempVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                //rb.velocity = vectorPasser(tempVelocity.normalized * airMaxVelocity, rb.velocity.y);
            }
        }
        else
        {
            if (actualSpeed > groundMaxVelocity)
                actualSpeed = groundMaxVelocity;

            if (vectorPasser(rb.velocity, 0).magnitude > groundMaxVelocity)
            {
                Vector3 tempVelocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                rb.velocity = vectorPasser(tempVelocity.normalized * groundMaxVelocity, 0);
            }
        }

        Vector3 moveVector = transform.TransformDirection(smartPlayerMovementInput);
        
        Vector3 airVector = transform.TransformDirection(rawPlayerMovementInput);


        if (feet.isGrounded)
        {
            var ray = new Ray(transform.position + Vector3.down * (halfOfHeight), Vector3.down);

            if (Physics.Raycast(ray, out floorInfo, 0.1f))
            {
                if (floorInfo.normal != Vector3.up)
                {
                    isOnSlope = true;
                    /*
                    var slopeRotation = Quaternion.FromToRotation(Vector3.up, floorInfo.normal);
                    var adjustedDirection = slopeRotation * slopeMoveVector;
                    
                    rb.velocity = adjustedDirection * actualSpeed;*/

                    slopeMoveVector = Vector3.ProjectOnPlane(moveVector, floorInfo.normal);

                    if (rawPlayerMovementInput.magnitude == 0)
                    {
                        //cancel friction
                        rb.velocity = Vector3.zero;
                        return;
                    }

                    camHolder.localPosition = Vector3.zero;

                    if (slopeMoveVector.y < 0)
                        rb.velocity = new Vector3(slopeMoveVector.x * actualSpeed, slopeMoveVector.y * actualSpeed, slopeMoveVector.z * actualSpeed);
                    else if (slopeMoveVector.y > 0)
                        rb.velocity = new Vector3(slopeMoveVector.x * actualSpeed, slopeMoveVector.y, slopeMoveVector.z * actualSpeed);
                    

                    return;
                }
                else isOnSlope = false;
            }

            rb.velocity = new Vector3(moveVector.x * actualSpeed, rb.velocity.y, moveVector.z * actualSpeed);
        }
        else
        {
            //rb.AddForce(airVector * airControl, ForceMode.VelocityChange);

            if (airVector.magnitude == 0)
            {
                return;
            }

            //rb.velocity = new Vector3((airVector.x * airControl * actualSpeed), rb.velocity.y, (airVector.z * airControl * actualSpeed));

            float momentum = 1 - airControl;
            rb.velocity = new Vector3((momentum * rb.velocity.x) + (airVector.x * airControl * actualSpeed), rb.velocity.y, (momentum * rb.velocity.z) + (airVector.z * airControl * actualSpeed));
        }
    }

    void SlopeHelper()
    {
        if (smartPlayerMovementInput.magnitude != 0)
        {
            Physics.Raycast(colBottom, Vector3.down, 1.5f);

            rb.velocity += Vector3.down * 10;
            print("active");
        }
    }

    private Vector3 vectorPasser(Vector3 input, float preferredY)
    {
        return new Vector3(input.x, preferredY, input.x);
    }

    #endregion

    #region Skills
    IEnumerator Dash(Vector3 dir)
    {
        if (canDash)
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

            rb.velocity = rb.velocity * 0.2f;
            dashRecovery.PlayFeedbacks();
        }
    }

    public void TeleportTo()
    {
        if (!shuriken.state.Equals(FumaState.InHands)) shuriken.Returned(Vector3.zero);

        transform.position = shuriken.teleportLocation;
        rb.velocity = Vector3.up * 2;
        isJumping = false;
        isTeleporting = false;
    }

    public void Melee()
    {
        //put delay for the animation;
        meleeCounter = meleeCooldown;
        isMeleeing = false;
        playerAnimator.SetBool("isMeleeing", isMeleeing);
    }

    #endregion

    IEnumerator Knocked(float knockbackForce)
    {
        //Debug.Log(dir);
        //rb.AddForce((dir + Vector3.up) * dashForce, ForceMode.Impulse);
        //rb.AddForce(Vector3.back * knockbackForce, ForceMode.Impulse);
        yield return new WaitForSeconds(knockbackRecoveryTime);
        isKnocked = false;
    }

}
