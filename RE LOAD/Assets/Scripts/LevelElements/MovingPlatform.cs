using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 startingPos, lastPos;
    public Vector3 targetPos;
    public float speed;

    public void Start()
    {
        startingPos = transform.position;
        lastPos = startingPos;
        rb = GetComponent<Rigidbody>();
    }

    public void Update()
    {
        if (targetPos != null)
        {
            Vector3 direction = targetPos - startingPos;

            if (lastPos == startingPos)
            {
                rb.transform.Translate(direction.normalized * speed);

                if (transform.position == targetPos)
                {
                    lastPos = targetPos;
                }
            }
            else
            {
                rb.transform.Translate(-direction.normalized * speed);

                if (transform.position == startingPos)
                {
                    lastPos = startingPos;
                }
            }
            
        }
    }


    private void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Player")
		{
			other.transform.parent = transform;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Player")
		{
			other.transform.parent = null;
		}
	}
}
