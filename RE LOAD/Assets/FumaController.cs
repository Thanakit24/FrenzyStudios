using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum FumaState
{
    InHands,
    Flying,
    Returning,
    Ragdoll,
    Stuck
}

public class FumaController : MonoBehaviour
{
    public FumaState state = FumaState.InHands;
    public Transform player, holder;
    

    public Vector3 curveRot, throwRotation;
    public float flyingSpeed, chargeSpeed, maxBounces, ySpinSpeed, xSpinSpeed, pickupRange, destroyDistance, fxDestroyTime, ragdollSpin;
    public bool curvedStart, curvedFlying, curvedReturn;

    MeshCollider col;
    Vector3 lastPos, returnPos;
    Rigidbody rb;
    Transform model, cam;
    GameObject impactFX, trailFX, modelOBJ;
    bool mustReturn = true, firstBounce;
    float tempBounces;
    int bounces;

    public TextMeshProUGUI text;

    void Start()
    {
        cam = Camera.main.transform;
        col = GameObject.Find("ShurikenMesh").GetComponent<MeshCollider>();
        model = GameObject.Find("ShurikenMeshContainer").transform;
        modelOBJ = GameObject.Find("ShurikenMesh");
        impactFX = GameObject.Find("ImpactFX");
        trailFX = GameObject.Find("TrailFX");
        rb = GetComponent<Rigidbody>();

        Returned();

        impactFX.GetComponent<ParticleSystem>().playOnAwake = true;
    }

    void Update()
    {
        if (state.Equals(FumaState.InHands))
        {
            if (Input.GetKey(KeyCode.Mouse0))
            {
                tempBounces += chargeSpeed * Time.deltaTime;
                bounces = Mathf.RoundToInt(tempBounces);

                if (bounces >= maxBounces)
                    Throw();
            }
            else if (Input.GetKeyUp(KeyCode.Mouse0)) Throw();

            rb.velocity = Vector3.zero;
        }
        else if (state.Equals(FumaState.Flying) || state.Equals(FumaState.Returning))
        {
            if (!player) Returned();

            //Movement
            //transform.Translate(transform.forward * flyingSpeed * Time.deltaTime, Space.World);
            //Rotation();

            if (Input.GetKeyUp(KeyCode.Mouse0)) mustReturn = true;
        }
        else if (state.Equals(FumaState.Ragdoll) || state.Equals(FumaState.Stuck))
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance < pickupRange) Returned();
        }

        if (player)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance > destroyDistance) Returned();
        }
        else Returned();

        if (text != null )text.text = bounces.ToString();
    }

    private void FixedUpdate()
    {
        if (state.Equals(FumaState.Flying) || state.Equals(FumaState.Returning))
        {
            //Movement
            transform.Translate(transform.forward * flyingSpeed * Time.deltaTime, Space.World);
            Rotation();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        bool isPlayer = (collision.collider.CompareTag("Player") || collision.collider.CompareTag("LeftHand"));

        if (!state.Equals(FumaState.InHands))
        {
            if (collision.transform.CompareTag("Sticky")) Stick();
            if (isPlayer && firstBounce) Returned();
        }

        if (state.Equals(FumaState.Flying) && bounces > 0 && !isPlayer)
        {
            //Debug.Log(collision.collider.name);
            Bounce(collision.GetContact(0).normal);
        }

        if (state.Equals(FumaState.Returning)) Ragdoll();
    }

    void Bounce(Vector3 contactNormalDirection)
    {
        firstBounce = true;
        bounces -= 1;

        Vector3 direction = contactNormalDirection - lastPos.normalized;
        direction = direction.normalized;
        transform.rotation = Quaternion.LookRotation(direction, Vector3.forward);
        lastPos = transform.position;

        var fx = Instantiate(impactFX, transform.position, Quaternion.identity);
        fx.SetActive(true);
        Destroy(fx, fxDestroyTime);

        rb.useGravity = false;

        if (mustReturn)
        {
            state = FumaState.Returning;

            bounces = 1;

            returnPos = player.position;
            transform.LookAt(returnPos);
        }
        else if (bounces <= 0) Ragdoll();
    }

    void Stick()
    {
        state = FumaState.Stuck;

        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.useGravity = false;
        rb.isKinematic = true;
        mustReturn = false;

        trailFX.SetActive(false);
        col.enabled = false;
    }

    void Rotation()
    {
        model.RotateAround(model.position, model.up, ySpinSpeed * Time.deltaTime);

        if (!curvedStart && !firstBounce)
        {
            transform.eulerAngles = transform.eulerAngles + (curveRot * Time.deltaTime);
        }
        else if (curvedFlying && state.Equals(FumaState.Flying))
        {
            transform.eulerAngles = transform.eulerAngles + (curveRot * Time.deltaTime);
        }
        else if (curvedFlying && state.Equals(FumaState.Returning))
        {
            transform.eulerAngles = transform.eulerAngles + (curveRot * Time.deltaTime);
        }
        else 
        {
            model.RotateAround(transform.position, transform.forward, xSpinSpeed * Time.deltaTime);
        }
    }

    void Throw()
    {
        firstBounce = false;
        transform.SetParent(null);
        transform.position = cam.position + cam.forward;
        transform.eulerAngles = cam.eulerAngles + throwRotation;
        trailFX.SetActive(true);
        col.enabled = true;
        rb.isKinematic = false;

        state = FumaState.Flying;
    }

    void Returned()
    {
        if (!player) return;

        state = FumaState.InHands;

        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.useGravity = false;
        rb.isKinematic = true;

        mustReturn = false;
        trailFX.SetActive(false);
        col.enabled = false;
        transform.SetParent(holder);
        transform.localPosition = Vector3.zero;
        model.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(0,0,0);
        model.localRotation = Quaternion.Euler(0,0,0);
        bounces = 1;
        tempBounces = 1;
    }

    void Ragdoll()
    {
        if (state.Equals(FumaState.InHands)) return;

        state = FumaState.Ragdoll;

        //rb.constraints = RigidbodyConstraints.None;
        rb.useGravity = true;
        rb.isKinematic = false;
        Vector3 torque = new Vector3(Random.Range(-ragdollSpin, ragdollSpin), Random.Range(-ragdollSpin, ragdollSpin), Random.Range(-ragdollSpin, ragdollSpin));
        rb.AddTorque(torque);

        var fx = Instantiate(impactFX, transform.position, Quaternion.identity);
        fx.SetActive(true);
        Destroy(fx, fxDestroyTime);

        trailFX.SetActive(false);
    }
}
