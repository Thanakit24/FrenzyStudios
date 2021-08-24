using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feet : MonoBehaviour
{
    BoxCollider feet;
    [HideInInspector] public bool isGrounded;

    private void Start()
    {
        feet = GetComponent<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        isGrounded = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isGrounded = false;
    }

}
