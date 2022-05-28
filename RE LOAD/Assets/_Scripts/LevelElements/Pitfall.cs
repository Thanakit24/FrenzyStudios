using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pitfall : MonoBehaviour
{
    public Transform newPos;

	[SerializeField] private Image white;

	private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			other.transform.position = newPos.position;
			StartCoroutine(FadeOut());
		}
	}

	IEnumerator FadeOut()
	{
		white.gameObject.SetActive(true);
		for(float f = 1f; f >= -0.05; f-= 0.05f)
		{
			Color c = white.GetComponent<Image>().color;
			c.a = f;
			white.GetComponent<Image>().color = c;
			yield return new WaitForSeconds(0.05f);
		}
		white.gameObject.SetActive(false);
	}
}
