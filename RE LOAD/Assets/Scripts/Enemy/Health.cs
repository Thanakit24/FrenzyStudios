using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Health : MonoBehaviour
{
    public int health;
    [SerializeField] private Animator anim;
    [SerializeField] private GuardianController gc;
    [SerializeField] private DeathDelete dd;

    Collider col;

	private void Start()
	{
        anim = GetComponentInChildren<Animator>();
        gc = GetComponentInChildren<GuardianController>();
        dd = GetComponent<DeathDelete>();
        col = GetComponent<Collider>();
	}

	public void TakeDamage(int damage)
    { 
        health -= damage;

        if (health <= 0)
        {
            gameObject.GetComponent<NavMeshAgent>().isStopped = true;
            col.enabled = false;
            dd.DeathDestroy();
            gc.enabled = false;
            anim.enabled = false;
        }
    }
}
