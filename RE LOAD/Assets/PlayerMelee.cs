using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMelee : MonoBehaviour
{
    public Vector3 meleeKnockback;
    public int damage;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || !other.CompareTag("Shuriken"))
        {

            Rigidbody rb = other.GetComponent<Rigidbody>();
            Health hp = other.GetComponent<Health>();

            if (rb != null)
            {
                Debug.Log(other.name);

                rb.AddForce(transform.forward * meleeKnockback.z + Vector3.up * meleeKnockback.y, ForceMode.Impulse);
            }
            if (hp != null)
            {
                if (other.CompareTag("Enemy"))
                {
                    Debug.Log("Enemy Found");
                }

                hp.TakeDamage(damage);
            }
        }
    }
}
