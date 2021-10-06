using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardianShield : MonoBehaviour
{
	[SerializeField] private int shieldDamage;
	[SerializeField] private float shieldRange;


	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			Debug.Log("Player in shield range.");

			Collider[] damageRadius = Physics.OverlapSphere(transform.position, shieldRange);
			for(int i = 0; i < damageRadius.Length; i++)
			{
				PlayerHealth playerHP = damageRadius[i].GetComponent<PlayerHealth>();
				if(playerHP != null)
				{
					Debug.Log("Making shield damage.");
					playerHP.TakeDamage(shieldDamage);

					//other.GetComponent<Rigidbody>();
					
				}
			}

		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, shieldRange);
	}
}
