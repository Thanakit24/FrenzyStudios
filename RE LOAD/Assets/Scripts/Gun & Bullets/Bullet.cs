using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;  
    [Range(0f,1f)]
    [SerializeField] private bool useGravity;
    public int damage;
    private PhysicMaterial physics_mat;

    [Header("Return Settings")]
    [SerializeField] private bool isReturning;
    [SerializeField] private GameObject hand;
    [SerializeField] private Gun gun;
    [SerializeField] private Vector3 dirToTarget;
    private GameObject target;
    private bool hasTarget;
    private float destroyRange;
    private float returnSpeed;
    private float speedBoost;
    private float maxSpeed;
    private bool wasFired;

    private void Start()
    {
        Setup();
        isReturning = false;
        hand = GameObject.Find("Recall Point");
        gun = GameObject.Find("Gun").GetComponent<Gun>();
        returnSpeed = gun.returnSpeed;
        destroyRange = gun.destroyRange;
        speedBoost = 0;
        maxSpeed = gun.maxReturnSpeed;
        hasTarget = false;
    }

    private void FixedUpdate()
    {
        if (isReturning)
        {
            wasFired = true;
            useGravity = false;
            rb.constraints = RigidbodyConstraints.None;
            SelectTarget();
            //dirToTarget = (hand.transform.position - transform.position).normalized;
            
            if (speedBoost < maxSpeed)
            {
                speedBoost += (maxSpeed - returnSpeed)/10 * 0.01f* (2+ speedBoost);
            }

            rb.velocity = dirToTarget * (returnSpeed + speedBoost) * Time.deltaTime;
            transform.LookAt(hand.transform, Vector3.up);

            if ((hand.transform.position - transform.position).magnitude < destroyRange)
            {
                gun.AddBullet();
                Destroy(gameObject);
            }
        }
        else
        {
            if (gun.holdToReturn && wasFired)
            {
                rb.useGravity = true;
            }
        }
        
    }

    private void SelectTarget()
    {
        if (target != null && hasTarget)
        {
            dirToTarget = (target.transform.position - transform.position).normalized;
        }
        else
        {
            dirToTarget = (hand.transform.position - transform.position).normalized;
        }
    }

    public void AddTarget(GameObject enemy)
    {
        target = enemy;
        hasTarget = true;
    }

    private void Setup()
    {
        //Create a new Physic material
        physics_mat = new PhysicMaterial();
        //physics_mat.bounciness = bounciness;
        physics_mat.frictionCombine = PhysicMaterialCombine.Minimum;
        physics_mat.bounceCombine = PhysicMaterialCombine.Maximum;
        //Assign material to collider
        GetComponent<BoxCollider>().material = physics_mat;

        //Set gravity
        rb.useGravity = useGravity;
    }

    public void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject.name == "Left Hand")
        {
            isReturning = false;
            Destroy(gameObject);

            GameObject.Find("Gun").GetComponent<Gun>().AddBullet();
            Destroy(gameObject);
        }

        if (collider.gameObject.tag != "Enemy")
        {
            
            if (collider.gameObject.CompareTag("Immovable"))
            {
                rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

            }

        }
        else
        {
            Health enemy = collider.gameObject.GetComponent<Health>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }

        
    }




    public void Recall()
    {
        //Debug.Log("Recalling");
        isReturning = true;
    }

    public void CancelRecall()
    {
        isReturning = false;
    }
}
