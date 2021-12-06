using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Health : MonoBehaviour
{
    public int health;
    public AnimController ac;
    public GuardianController gc;

	private void Start()
	{
        ac = GetComponentInChildren<AnimController>();
        gc = GetComponentInChildren<GuardianController>();
	}

	public void TakeDamage(int damage)
    { 
        health -= damage;

        if (health <= 0)
        {
            gameObject.GetComponent<NavMeshAgent>().isStopped = true;
            gc.enabled = false;
            ac.anim.SetTrigger("Dead");
            //Destroy(gameObject);
        }
    }
}
