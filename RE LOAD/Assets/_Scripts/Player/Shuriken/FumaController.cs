using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MoreMountains.Feedbacks;
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
    public StanceController stance;
    public KeyCode shootingKey;
    public bool canShoot;

    [Header("References")]
    public FumaState state = FumaState.InHands;
    public Transform player, holder;
    public SeeThroughWall stw;
    public MMFeedbacks bounceSFX;
    public GameObject prefab;
    public List<GameObject> lines;
    public int linesShown = 2;
    public int damage;

    [HideInInspector] public Vector3 teleportLocation;
    public float teleportLocationCounter;

    [Header("Config 1")]
    public Vector3 curveRot, throwRotation;
    public float flyingSpeed, chargeSpeed, maxBounces, ySpinSpeed, xSpinSpeed, pickupRange, destroyDistance, fxDestroyTime, ragdollSpin;
    public bool curvedStart, curvedFlying, curvedReturn;
    [HideInInspector] public bool shouldLockOnToPlayer;

    [Header("Config 2")]
    public bool autoTeleportsToStickyInsteadofReturnShuriken = false;
    public bool disableLineRenderer = true;
    public TextMeshProUGUI text;
    public int RayCount = 2;

    [Header("General")]
    public MeshCollider col;
    Vector3 lastPos, returnPos;
    [HideInInspector] public Rigidbody rb;
    Transform model, cam;
    GameObject impactFX, trailFX, modelOBJ;
    bool mustReturn = true, firstBounce;
    public bool alwaysReturn = false, lockOnReturnToPlayer = true;
    float tempBounces;
    public int bounces;
    public int defaultBounces;
    [HideInInspector] public Vector3 returnVector;


    public Vector3 nextDir, nextPos;
    public Quaternion nextAngle;

    [Header("Debug")]
    public bool isElectrolyzed;

    [Header("Feedbacks")]
    public GameObject visualIndicator;
    public float cacheHeight;
    public MMFeedbacks differentHeightSoundQ;
    public MMFeedbacks throwFB;
    public MMFeedbacks recallFB;
    public Material safeTP;
    public Material dangerTP;
    public MMFeedbacks impactFB;

    public GameObject[] bounceUIprefabs;
    GameObject blueIndicator, greenIndicator;
    Vector3 greenIndicatorPos;

    public Color lineColor1, lineColor2;

    void Awake()
    {
        for (int i = 0; i < linesShown; i++)
        {
            CreateLine(transform.position);
        }

        cam = Camera.main.transform;
        //col = GameObject.Find("ShurikenMesh").GetComponent<MeshCollider>();
        model = GameObject.Find("ShurikenMeshContainer").transform;
        modelOBJ = GameObject.Find("ShurikenMesh");
        impactFX = GameObject.Find("ImpactFX");
        trailFX = GameObject.Find("TrailFX");
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        Returned(Vector3.zero);
        impactFX.GetComponent<ParticleSystem>().playOnAwake = true;

        shootingKey = stance.stanceChange;
    }

    void Update()
    {
        TeleportLocationDelaySystem();

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (state.Equals(FumaState.Flying) || state.Equals(FumaState.Ragdoll) || state.Equals(FumaState.Stuck))
            {
                state = FumaState.Returning;
                transform.SetParent(null);
                rb.constraints = RigidbodyConstraints.None;
                rb.isKinematic = false;

                recallFB.PlayFeedbacks();
                shouldLockOnToPlayer = true;
            }
        }

        VisualIndicatorSystem();

        if (state.Equals(FumaState.InHands))
        {
            if (Input.GetKey(shootingKey))
            {
                RepositionLine(Camera.main.transform.position, Camera.main.transform.forward, false);
                if (Input.GetKey(shootingKey))
                {
                    /*
                    tempBounces += chargeSpeed * Time.deltaTime;
                    bounces = Mathf.RoundToInt(tempBounces);

                    if (bounces >= maxBounces)
                        Throw();
                    */
                }
                //else if (Input.GetKeyUp(shootingKey)) Throw();

                //rb.velocity = Vector3.zero;
            }
            else if(Input.GetKeyUp(shootingKey)) Throw();
            {
                
                rb.velocity = Vector3.zero;

                ResetLine();
            }
        }
        else if (state.Equals(FumaState.Flying) || state.Equals(FumaState.Returning))
        {
            if (!player) Returned(Vector3.zero);

            trailFX.SetActive(true);

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
                if (distance < pickupRange) Returned((transform.position - player.position).normalized);
            }

            //Movement
            //transform.Translate(transform.forward * flyingSpeed * Time.deltaTime, Space.World);
            //Rotation();

            RepositionLine(transform.position, transform.forward, true);


            //if (Input.GetKeyUp(KeyCode.Mouse0)) mustReturn = true;
        }
        else if (state.Equals(FumaState.Ragdoll) || state.Equals(FumaState.Stuck))
        {
            float distance = Vector3.Distance(transform.position, player.position);
            ResetLine();
            if (distance < pickupRange) Returned((transform.position - player.position).normalized);
        }

        if (player)
        {
            float distance = Vector3.Distance(transform.position, player.position);
            if (distance > destroyDistance) Returned(Vector3.zero);
        }
        else Returned(Vector3.zero);

        if (text != null )text.text = bounces.ToString();
    }

    private void FixedUpdate()
    {
        if (state.Equals(FumaState.Flying) || state.Equals(FumaState.Returning))
        {
            //Movement
            //transform.Translate(transform.forward * flyingSpeed * Time.deltaTime, Space.World);
            rb.velocity = transform.forward * flyingSpeed;

            Rotation();

            if (state.Equals(FumaState.Returning) && lockOnReturnToPlayer)
            {
                transform.LookAt(player.position);
            }
        }
    }

    public void VisualIndicatorSystem()
    {
        if (state.Equals(FumaState.Flying) || state.Equals(FumaState.Returning))
        {
            visualIndicator.SetActive(true);
            RaycastHit hit;
            Physics.Raycast(transform.position, Vector3.down, out hit);
            MeshRenderer mr = visualIndicator.GetComponent<MeshRenderer>();

            /*
            Electrolyzed temp;
            temp = hit.collider.gameObject.GetComponent<Electrolyzed>();

            if (temp != null)
            {
                mr.material = dangerTP;
            }
            else
            {
                if (mr.material.name.ToString().Contains("TeleportVisual Danger"))
                {
                    differentHeightSoundQ.PlayFeedbacks();
                }
                mr.material = safeTP;
            }
            */

            visualIndicator.transform.position = hit.point;
            visualIndicator.transform.rotation = Quaternion.Euler(90, 0, 0);
        }
        else
        {
            visualIndicator.SetActive(false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (state.Equals(FumaState.Stuck)) return;


        bool isPlayer = (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Shuriken"));

        //Electricity
        Electrolyzed electrolyzed = collision.collider.GetComponent<Electrolyzed>();
        if (electrolyzed != null)
        {
            if (electrolyzed.input)
                isElectrolyzed = true;
            else
            {
                isElectrolyzed = false;
            }
            electrolyzed.OnContactActivation();
        }
        //Sticky
        if (!state.Equals(FumaState.InHands))
        {
            if (collision.transform.CompareTag("Sticky"))
            {
                Stick(collision.collider.gameObject.transform);
                impactFB.PlayFeedbacks();
            }
            if (isPlayer && firstBounce) Returned(Vector3.zero);
        }

        //Health
        Health hp = collision.collider.GetComponent<Health>();
        if (hp != null) hp.TakeDamage(damage);

        //Knockback + auto Tp
        if (!isPlayer)
        {
            Rigidbody rb = collision.collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                impactFB.PlayFeedbacks();
                teleportLocationCounter = .5f;
                teleportLocation = transform.position;

                rb.AddForce(-collision.GetContact(0).normal, ForceMode.Impulse);

                if (bounces <= 1 && autoTeleportsToStickyInsteadofReturnShuriken)
                {
                    /*
                    if (collision.transform.CompareTag("Sticky") || collision.transform.CompareTag("Enemy"))
                    {
                        PlayerController.instance.TeleportTo(transform.position);
                        PlayerController.instance.teleportWithSlowmoFB.PlayFeedbacks();
                    }
                    */
                }
            }
        }

        //Bounce
        if (state.Equals(FumaState.Flying) && bounces > 0 && !isPlayer && !collision.transform.CompareTag("Sticky"))
        {
            Bounce(collision.GetContact(0).normal, collision.GetContact(0).point);
        }

        //Ragdoll
        //if (state.Equals(FumaState.Returning)) Ragdoll();
    }
    

    void Bounce(Vector3 contactNormalDirection, Vector3 pos)
    {
        firstBounce = true;
        bounces -= 1;

        BounceUIIndicator(true);


        //COLLISION BOUNCE
        Vector3 direction = Vector3.Reflect(transform.forward, contactNormalDirection);
        transform.rotation = Quaternion.LookRotation(direction, Vector3.forward);

        //RAYCAST BOUNCE
        //transform.rotation = Quaternion.LookRotation(nextDir, Vector3.forward);
        //transform.LookAt(nextPos);
        //CastRayForBounce(transform.position, transform.forward);

        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0);
        lastPos = transform.position;

        var fx = Instantiate(impactFX, pos + contactNormalDirection * 0.1f, Quaternion.identity);
        fx.SetActive(true);
        Destroy(fx, fxDestroyTime);
        bounceSFX.PlayFeedbacks();

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

    void Stick(Transform newParent)
    {
        state = FumaState.Stuck;
        bounceSFX.PlayFeedbacks();

        transform.SetParent(newParent);
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

    public void Throw()
    {
        firstBounce = false;
        transform.SetParent(null);
        transform.position = cam.position + cam.forward;
        //transform.eulerAngles = cam.eulerAngles + (throwRotation * 5 * bounces /2);
        trailFX.SetActive(true);
        col.enabled = true;
        rb.isKinematic = false;

        

        throwFB.PlayFeedbacks();

        CastRayForBounce(transform.position, transform.forward);

        state = FumaState.Flying;
    }

    

    public void Returned(Vector3 returnVector)
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
        bounces = defaultBounces;
        tempBounces = 1;

        shouldLockOnToPlayer = false;

        if (blueIndicator != null)
        {
            Destroy(blueIndicator);
        }
        if (greenIndicator != null)
        {
            Destroy(greenIndicator);
        }
        ResetLine();

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
        //CastRay(transform.position, transform.forward);
        if (state.Equals(FumaState.Flying))
        {
            RaycastHit hit;
            Physics.Raycast(transform.position, Vector3.down, out hit);
            Debug.DrawLine(transform.position, hit.point);
        }
    }

    void CastRay(Vector3 pos, Vector3 dir)
    {
        for (int i = 0; i < maxBounces - bounces + 1; i++)
        {
            Ray ray = new Ray(pos, dir);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 15))
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

                if (i == 0)
                {
                    GameObject bounceIndicator = Instantiate(bounceUIprefabs[0], pos + hit.normal * 0.1f, Quaternion.identity, null);
                    blueIndicator = bounceIndicator;
                }

                if (i == 1)
                {
                    nextDir = dir;
                    nextPos = pos;

                    greenIndicatorPos = pos + hit.normal * 0.1f;

                    Electrolyzed temp;

                    if (hit.collider.CompareTag("Sticky") || hit.collider.CompareTag("Enemy") || hit.collider.TryGetComponent<Electrolyzed>(out temp))
                    {
                        GameObject bounceIndicator2 = Instantiate(bounceUIprefabs[1], pos + hit.normal * 0.1f, Quaternion.identity, null);
                        greenIndicator = bounceIndicator2;
                    }
                    
                }
            }
            else
            {
                break;
            }
        }
    }

    void CreateLine(Vector3 start)
    {
        GameObject newLine = Instantiate(prefab, start, Quaternion.identity);
        LineRenderer newLineRender = newLine.GetComponent<LineRenderer>();

        newLineRender.SetPosition(0, start);
        newLineRender.SetPosition(1, start);

        lines.Add(newLine);
    }

    public void RepositionLine(Vector3 pos, Vector3 dir, bool initialLineShouldDisplay)
    {
        if (linesShown == 0) return;

        for (int i = 0; i < maxBounces; i++)
        {
            Ray ray = new Ray(pos, dir);
            RaycastHit hit;

            LineRenderer lr = lines[0].GetComponent<LineRenderer>();
            LineRenderer lr2 = lines[1].GetComponent<LineRenderer>();

            if (Physics.Raycast(ray, out hit))
            {
                if (bounces <= 1)
{
                    ResetLine();
                    return;
                }

                //Debug.Log("bounces");

                if (i == 1)
                {
                    if (state.Equals(FumaState.Flying) && bounces -1 <=0)
                    {
                        ResetLine();
                        Vector3 dirToPlayer = pos - (player.transform.position - Vector3.up * 0.1f);
                        lr2.SetPosition(1, pos - dirToPlayer.normalized * 10);
                        lr2.SetPosition(0, pos);

                        lr2.startColor = lineColor2;
                        lr2.endColor = lineColor2;
                    }
                    else
                    {

                        //lr.SetPosition(1, pos + dir.normalized * 5);
                        lr.SetPosition(1, hit.point);
                        lr.SetPosition(0, pos);

                        lr.startColor = lineColor1;
                        lr.endColor = lineColor1;

                    }
                }

                

                if (i == maxBounces || i == 2)
                {
                    Vector3 dirToPlayer = pos - (player.transform.position - Vector3.up * 0.1f);
                    lr2.SetPosition(1, pos - dirToPlayer.normalized * 10);
                    lr2.SetPosition(0, pos);

                    lr2.endColor = lineColor2;
                    lr2.startColor = lineColor2;
                }

                if (state.Equals(FumaState.Returning))
                {
                    lr.SetPosition(0, transform.position);
                    lr.SetPosition(1, transform.position);
                }

                lr.endColor = new Color(lr.endColor.a, lr.endColor.g, lr.endColor.b, 0);
                lr2.endColor = new Color(lr2.endColor.a, lr2.endColor.g, lr2.endColor.b, 0);

                pos = hit.point;
                dir = Vector3.Reflect(dir, hit.normal);
            }
            else
            {
                lr.SetPosition(0, transform.position);
                lr.SetPosition(1, transform.position);
                break;
            }
        }
    }

    void ResetLine()
    {
        if (linesShown != 0)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                LineRenderer lr = lines[i].GetComponent<LineRenderer>();
                lr.SetPosition(0, transform.position);
                lr.SetPosition(1, transform.position);
            }
        }
    }

    public Vector3 GetTargetLocation()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, transform.forward, out hit);
        return hit.point;
    }

    void TeleportLocationDelaySystem()
    {
        if (teleportLocationCounter > 0)
        {
            teleportLocationCounter -= Time.deltaTime;
        }
        else
        {
            teleportLocation = transform.position;
        }
    }

    void BounceUIIndicator(bool enable)
    {
        if (enable)
        {
            if (bounces == 1)
            {
                blueIndicator.GetComponentInChildren<Animator>().Play("ShurikenCircleUI_Clockwise");

                if (greenIndicator == null)
                {
                    GameObject bounceIndicator2 = Instantiate(bounceUIprefabs[1], greenIndicatorPos, Quaternion.identity, null);
                    greenIndicator = bounceIndicator2;
                }
            }

            if (bounces == 0)
            {
                greenIndicator.GetComponentInChildren<Animator>().Play("ShurikenCircleUI_Counterclockwise");
            }
        }
    }
}
