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

    public int count;
    private int previousCount;
    private void Start()
    {
        feet = GetComponent<BoxCollider>();
        landingImpact =  GetComponentInParent<LandingImpact>();
        pc = GetComponentInParent<PlayerController>();
    }

    private void Update()
    {
        Collider[] touchingGrounds = Physics.OverlapSphere(transform.position + Vector3.down * .65f, .5f, whatIsGround);
        count = touchingGrounds.Length;
        if (previousCount < count && count == 1)
        {
            pc?.OnGrounded();
            landingImpact?.Activate();

            previousCount = count;
        }
        else if (previousCount > count)
            previousCount = count;

        if (count == 0)
        {
            landingImpact?.Deactivate();
            isGrounded = false;
        }
        else
        {
            //pc?.OnGrounded();
            //landingImpact?.Activate();
            isGrounded = true;
        }
    }
    /*
    private void OnTriggerStay(Collider other)
    {
        Collider[] touchingGrounds = Physics.OverlapSphere(transform.position + Vector3.down * .65f, .5f, whatIsGround);
        count = touchingGrounds.Length;

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
        count = touchingGrounds.Length;
        if (touchingGrounds.Length == 1)
        {
            landingImpact?.Deactivate();
            isGrounded = false;
        }
    }
    */
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position + Vector3.down * .65f, .5f);
    }
}
