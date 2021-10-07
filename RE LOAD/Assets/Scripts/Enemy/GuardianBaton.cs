using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardianBaton : MonoBehaviour
{
	[SerializeField] private int batonDamage;
	[SerializeField] private float batonRange;

	[SerializeField] private float knockback;

	private Rigidbody rb;

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			 rb = other.GetComponent<Rigidbody>();

			Collider[] damageRadius = Physics.OverlapSphere(transform.position, batonRange);
			for (int i = 0; i < damageRadius.Length; i++)
			{
				PlayerHealth playerHP = damageRadius[i].GetComponent<PlayerHealth>();
				if (playerHP != null)
				{
					playerHP.TakeDamage(batonDamage);

					rb.AddForce(other.transform.forward * knockback, ForceMode.Impulse);
				}
			}

		}
	}
}
