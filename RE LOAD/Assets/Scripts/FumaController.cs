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

    [Header("Config")]
    public Vector3 curveRot, throwRotation;
    public float flyingSpeed, chargeSpeed, maxBounces, ySpinSpeed, xSpinSpeed, pickupRange, destroyDistance, fxDestroyTime, ragdollSpin;
    public bool curvedStart, curvedFlying, curvedReturn;
    [HideInInspector] public bool shouldLockOnToPlayer;
    
    public MeshCollider col;
    Vector3 lastPos, returnPos;
    Rigidbody rb;
    Transform model, cam;
    GameObject impactFX, trailFX, modelOBJ;
    bool mustReturn = true, firstBounce;
    public bool alwaysReturn = false, lockOnReturnToPlayer = true;
    float tempBounces;
    public int bounces;
    public int defaultBounces;

    public TextMeshProUGUI text;

    public int RayCount = 2;

    public Vector3 nextDir, nextPos;
    public Quaternion nextAngle;

    [Header("Teleport Indicator")]
    public GameObject visualIndicator;
    public float cacheHeight;
    public MMFeedbacks differentHeightSoundQ;

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
        Returned();
        impactFX.GetComponent<ParticleSystem>().playOnAwake = true;

        shootingKey = stance.stanceChange;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && state.Equals(FumaState.Flying)|| state.Equals(FumaState.Ragdoll) || state.Equals(FumaState.Stuck))
        {
            shouldLockOnToPlayer = true;
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

            RepositionLine(transform.position, transform.forward, true);


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

    public void VisualIndicatorSystem()
    {
        if (state.Equals(FumaState.Flying) || state.Equals(FumaState.Returning))
        {
            visualIndicator.SetActive(true);
            Ray ray;
            RaycastHit hit;
            Physics.Raycast(transform.position, Vector3.down, out hit);

            if (Vector3.Magnitude(hit.point - GetTargetLocation()) < 10)
            {
                if (cacheHeight != hit.point.y)
                {
                    cacheHeight = hit.point.y;
                    differentHeightSoundQ.PlayFeedbacks();
                }
            }

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

            Bounce(collision.GetContact(0).normal, collision.GetContact(0).point);
            //Bounce();
        }

        //if (state.Equals(FumaState.Returning)) Ragdoll();
    }
    

    void Bounce(Vector3 contactNormalDirection, Vector3 pos)
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

        var fx = Instantiate(impactFX, pos + contactNormalDirection, Quaternion.identity);
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

    public void Throw()
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
        bounces = defaultBounces;
        tempBounces = 1;

        shouldLockOnToPlayer = false;

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
            Ray ray;
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

                        lr2.startColor = Color.green;
                        lr2.endColor = Color.green;
                    }
                    else
                    {

                        //lr.SetPosition(1, pos + dir.normalized * 5);
                        lr.SetPosition(1, hit.point);
                        lr.SetPosition(0, pos);

                        lr.startColor = Color.red;
                        lr.endColor = Color.red;

                    }
                }

                

                if (i == maxBounces || i == 2)
                {
                    Vector3 dirToPlayer = pos - (player.transform.position - Vector3.up * 0.1f);
                    lr2.SetPosition(1, pos - dirToPlayer.normalized * 10);
                    lr2.SetPosition(0, pos);

                    lr2.endColor = Color.green;
                    lr2.startColor = Color.green;
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
        for (int i = 0; i < lines.Count; i++)
        {
            LineRenderer lr = lines[i].GetComponent<LineRenderer>();
            lr.SetPosition(0, transform.position);
            lr.SetPosition(1, transform.position);
        } 
    }

    public Vector3 GetTargetLocation()
    {
        Ray ray;
        RaycastHit hit;
        Physics.Raycast(transform.position, transform.forward, out hit);
        return hit.point;
    }
}
