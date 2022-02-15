using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 startingPos;
    public Vector3 targetPos;
    public float speed;
    public float snap;
    public bool isGoing;

    public void Start()
    {
        startingPos = transform.position;
        isGoing = true;
    }

    public void Update()
    {
        Vector3 direction = targetPos - startingPos;

        if (isGoing)
        {
            float magnitude = Vector3.Magnitude(transform.position - targetPos);

            transform.Translate(direction.normalized * speed * Time.deltaTime);

            if (magnitude < snap)
            {
                transform.position = targetPos;
                isGoing = false;
            }
        }
        else
        {
            float magnitude = Vector3.Magnitude(transform.position - startingPos);

            transform.Translate(-direction.normalized * speed * Time.deltaTime);

            if (magnitude < snap)
            {
                transform.position = startingPos;
                isGoing = true;
            }
        }
    }


    private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
            PlayerController.instance.transform.parent = transform;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
            PlayerController.instance.transform.parent = null;
        }
    }
}
