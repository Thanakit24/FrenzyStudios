using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            gc.enabled = false;
            ac.anim.SetTrigger("Dead");
            //Destroy(gameObject);
        }
    }
}
