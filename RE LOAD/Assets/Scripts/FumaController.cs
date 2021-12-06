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

    public int damage;

    public Vector3 curveRot, throwRotation;
    public float flyingSpeed, chargeSpeed, maxBounces, ySpinSpeed, xSpinSpeed, pickupRange, destroyDistance, fxDestroyTime, ragdollSpin;
    public bool curvedStart, curvedFlying, curvedReturn;
    private bool shouldLockOnToPlayer;
    
    public MeshCollider col;
    Vector3 lastPos, returnPos;
    Rigidbody rb;
    Transform model, cam;
    GameObject impactFX, trailFX, modelOBJ;
    bool mustReturn = true, firstBounce;
    public bool alwaysReturn = false, lockOnReturnToPlayer = true;
    float tempBounces;
    public int bounces;

    public TextMeshProUGUI text;

    public int RayCount = 2;

    public Vector3 nextDir, nextPos;
    public Quaternion nextAngle;

    void Start()
    {
        cam = Camera.main.transform;
        //col = GameObject.Find("ShurikenMesh").GetComponent<MeshCollider>();
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
        if (Input.GetKeyDown(KeyCode.R) && state.Equals(FumaState.Flying)|| state.Equals(FumaState.Ragdoll) || state.Equals(FumaState.Stuck))
        {
            shouldLockOnToPlayer = true;


        }

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


            float distance = Vector3.Distance(transform.position, player.position);

            if (shouldLockOnToPlayer && distance > pickupRange)
            {
                state = FumaState.Returning;

                bounces = 1;

                returnPos = player.position;
                transform.LookAt(returnPos);
            }
            
            if (state.Equals(FumaState.Returning))  
            {
                if (distance < pickupRange) Returned();
            }

            //Movement
            //transform.Translate(transform.forward * flyingSpeed * Time.deltaTime, Space.World);
            //Rotation();

            //if (Input.GetKeyUp(KeyCode.Mouse0)) mustReturn = true;
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
            transform.Translate(transform.forward * flyingSpeed * Time.deltaTime, Space.World);  //I see why u r using this but its probably why the shuriken going


            //through wall bug exists, do look into it later
            Rotation();

            if (state.Equals(FumaState.Returning) && lockOnReturnToPlayer)
            {
                transform.LookAt(player.position);
            }
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

            if (collision.collider.CompareTag("Enemy"))
            {
                collision.collider.GetComponent<Health>().TakeDamage(damage);
            }

            Bounce(collision.GetContact(0).normal);
            //Bounce();
        }

        //if (state.Equals(FumaState.Returning)) Ragdoll();
    }
    

    void Bounce(Vector3 contactNormalDirection)
    {
        firstBounce = true;
        bounces -= 1;

        //COLLISION BOUNCE
        Vector3 direction = Vector3.Reflect(transform.forward, contactNormalDirection);
        transform.rotation = Quaternion.LookRotation(direction, Vector3.forward);

        //RAYCAST BOUNCE
        //transform.rotation = Quaternion.LookRotation(nextDir, Vector3.forward);
        //transform.LookAt(nextPos);
        //CastRayForBounce(transform.position, transform.forward);

        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0);
        lastPos = transform.position;

        var fx = Instantiate(impactFX, transform.position, Quaternion.identity);
        fx.SetActive(true);
        Destroy(fx, fxDestroyTime);

        rb.useGravity = false;

        if (alwaysReturn)
        {
            state = FumaState.Returning;
            return;
        }


        if (mustReturn)
        {
            state = FumaState.Returning;

            bounces = 1;

            returnPos = player.position;
            transform.LookAt(returnPos);
        }
        else if (bounces <= 0)
        {
            if (alwaysReturn)
            {
                state = FumaState.Returning;

                returnPos = player.position;
                transform.LookAt(returnPos);

                bounces = 1;
            }
            else state = FumaState.Returning;
            //else Ragdoll();
        }
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
            transform.eulerAngles = transform.eulerAngles + (curveRot * 5 * bounces * Time.deltaTime);
        }
        else if (curvedFlying && state.Equals(FumaState.Flying))
        {
            transform.eulerAngles = transform.eulerAngles + (curveRot * 5 * bounces * Time.deltaTime);
        }
        else if (curvedFlying && state.Equals(FumaState.Returning))
        {
            transform.eulerAngles = transform.eulerAngles + (curveRot * 5 * bounces * Time.deltaTime);
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
        //transform.eulerAngles = cam.eulerAngles + (throwRotation * 5 * bounces /2);
        trailFX.SetActive(true);
        col.enabled = true;
        rb.isKinematic = false;

        CastRayForBounce(transform.position, transform.forward);

        state = FumaState.Flying;
    }

    public void Returned()
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

        shouldLockOnToPlayer = false;
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

    private void OnDrawGizmos()
    {
        CastRay(transform.position, transform.forward);
        //Debug.DrawLine(transform.position, nextPos, Color.green);
    }

    void CastRay(Vector3 pos, Vector3 dir)
    {
        for (int i = 0; i < maxBounces - bounces + 1; i++)
        {
            Ray ray = new Ray(pos, dir);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 10))
            {
                Debug.DrawLine(pos, hit.point, Color.red);

                pos = hit.point;
                dir = Vector3.Reflect(dir, hit.normal);
            }
            else
            {
                Debug.DrawRay(pos, dir * 10, Color.blue);
                break;
            }
        }
    }

    void CastRayForBounce(Vector3 pos, Vector3 dir)
    {
        for (int i = 0; i < 2; i++)
        {
            Ray ray = new Ray(pos, dir);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 50))
            {
                pos = hit.point;
                dir = Vector3.Reflect(dir, hit.normal);

                if (i == 1)
                {
                    nextDir = dir;
                    nextPos = pos;
                }
            }
            else
            {
                break;
            }
        }
    }
}
