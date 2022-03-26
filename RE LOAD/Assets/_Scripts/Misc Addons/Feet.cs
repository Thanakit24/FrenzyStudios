using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Feet : MonoBehaviour
{
    BoxCollider feet;
    public bool isGrounded;
    public LayerMask whatIsGround;
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
        if (!isGrounded && Physics.CheckSphere(transform.position + Vector3.down * .65f, .5f, whatIsGround))
        {
            pc?.OnGrounded();
            landingImpact?.Activate();
        }
        isGrounded = true;
    }

    private void OnTriggerExit(Collider other)
    {
        Collider[] touchingGrounds = Physics.OverlapSphere(transform.position + Vector3.down * .65f, .5f, whatIsGround);

        if (touchingGrounds.Length == 1)
        {
            Debug.Log("Hey");
            landingImpact?.Deactivate();
            isGrounded = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position + Vector3.down * .65f, .5f);
    }
}
