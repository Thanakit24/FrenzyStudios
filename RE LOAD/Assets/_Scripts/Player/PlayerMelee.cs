using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class PlayerMelee : MonoBehaviour
{
    public Vector3 meleeKnockback;
    public int damage;
    public MMFeedbacks meleeImpact;

    BoxCollider col;


    private void Start()
    {
        col = GetComponent<BoxCollider>();
    }
    /*
    private void Update()
    {
        Collider[] hits = Physics.OverlapBox(col.center, col.extents/2);

        foreach (var item in hits)
        {
            print(item.name);
            Health hp = item.GetComponent<Health>();
            if (hp != null) hp.TakeDamage(damage);
        }
    }
    */


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || !other.CompareTag("Shuriken"))
        {
            print(other.name);
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                if (other.gameObject.layer == 14)
                {
                    rb.velocity = Vector3.zero;
                    other.GetComponent<EnemyBullet>().hurtEnemy = true;
                    rb.AddForce(transform.forward * 25, ForceMode.Impulse);
                    meleeImpact.PlayFeedbacks();
                }
                else
                {
                    rb.velocity = Vector3.zero;
                    rb.AddForce(transform.forward * meleeKnockback.z + Vector3.up * meleeKnockback.y, ForceMode.Impulse);
                    meleeImpact.PlayFeedbacks();
                }
            }

            Health hp = other.GetComponent<Health>();
            if (hp != null)
            {
                print("fuck");
                hp.TakeDamage(1000000);
            }
        }

        
    }

    /*
    private void OnTriggerStay(Collider other)
    {
        if (!other.CompareTag("Player") || !other.CompareTag("Shuriken"))
        {
            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                if (other.gameObject.layer == 14)
                {
                    rb.velocity = Vector3.zero;
                    other.GetComponent<EnemyBullet>().hurtEnemy = true;
                    rb.AddForce(transform.forward * 25, ForceMode.Impulse);
                    meleeImpact.PlayFeedbacks();
                }
                else
                {
                    rb.velocity = Vector3.zero;
                    rb.AddForce(transform.forward * meleeKnockback.z + Vector3.up * meleeKnockback.y, ForceMode.Impulse);
                    meleeImpact.PlayFeedbacks();
                }
            }

            if (other.GetComponent<EnemyController>() != null)
            {
                print("fdsafdsa");
            }


            Health hp = other.GetComponent<Health>();
            if (hp != null)
            {
                hp.TakeDamage(damage);
            }
        }
    }
    */
}
