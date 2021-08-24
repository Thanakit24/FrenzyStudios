using UnityEngine;

public class Bullet : MonoBehaviour
{
    //Assignables
    public Rigidbody rb;
    public LayerMask whatIsEnemies;

    //Stats
    [Range(0f,1f)]
    public float bounciness;
    public bool useGravity;

    //Damage
    public int damage;

    PhysicMaterial physics_mat;

    private void Start()
    {
        Setup();
    }


    private void Delay()
    {
        Destroy(gameObject);
    }

    private void Setup()
    {
        //Create a new Physic material
        physics_mat = new PhysicMaterial();
        physics_mat.bounciness = bounciness;
        physics_mat.frictionCombine = PhysicMaterialCombine.Minimum;
        physics_mat.bounceCombine = PhysicMaterialCombine.Maximum;
        //Assign material to collider
        GetComponent<BoxCollider>().material = physics_mat;

        //Set gravity
        rb.useGravity = useGravity;
    }

    public void OnCollisionEnter(Collision collider)
    {
        if (collider.gameObject.tag != "Enemy")
        {
            Destroy(gameObject);
        }

        Health enemy = collider.gameObject.GetComponent<Health>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }
    }
}
