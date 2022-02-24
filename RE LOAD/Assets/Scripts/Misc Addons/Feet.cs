using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feet : MonoBehaviour
{
    BoxCollider feet;
    public bool isGrounded;
    private LandingImpact landingImpact;
    private PlayerController pc;

    private void Start()
    {
        feet = GetComponent<BoxCollider>();
        landingImpact =  GetComponentInParent<LandingImpact>();
        pc = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (!isGrounded)
        {
            pc.OnGrounded();
            landingImpact?.Activate();
        }
        isGrounded = true;
    }

    private void OnTriggerExit(Collider other)
    {
        landingImpact?.Deactivate();
        isGrounded = false;
    }

}
