 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    //[Range(0f, 1f)]
    [SerializeField] private bool useGravity;
    public int damage;
    private PhysicMaterial physics_mat;

    // Start is called before the first frame update
    private void Start()
    {
        Setup();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider collider)
    {
        print(collider.gameObject.ToString());
        if (collider.gameObject.tag == "Player")
        {
            print("fds");
            PlayerController player = collider.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                print("fdsdfsafdsa");
                player.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }

    private void Setup()
    {
        //Create a new Physic material
        physics_mat = new PhysicMaterial();
        physics_mat.frictionCombine = PhysicMaterialCombine.Minimum;
        physics_mat.bounceCombine = PhysicMaterialCombine.Maximum;

        //Assign material to collider
        GetComponent<BoxCollider>().material = physics_mat;

        //Set gravity
        rb.useGravity = useGravity;
    }
}
