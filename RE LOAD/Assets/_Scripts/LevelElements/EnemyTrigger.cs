using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{
    public GameObject[] enemies;

	private void OnTriggerEnter(Collider other)
	{
		foreach (GameObject gaob in enemies)
		{
			gaob.SetActive(true);
		}

		Destroy(this.gameObject);
	}
}
