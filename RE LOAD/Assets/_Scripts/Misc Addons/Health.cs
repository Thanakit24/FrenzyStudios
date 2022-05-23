using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Health : MonoBehaviour
{
    public int health;
    [SerializeField] private Animator anim;

    Collider col;

	private void Start()
	{
        anim = GetComponentInChildren<Animator>();
        col = GetComponent<Collider>();
	}

	public void TakeDamage(int damage)
    { 
        health -= damage;

        if (health <= 0)
        {
            gameObject.GetComponent<NavMeshAgent>().isStopped = true;
            col.enabled = false;
            Destroy(gameObject);
        }
    }
}
