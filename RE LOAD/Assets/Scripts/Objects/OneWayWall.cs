using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayWall : MonoBehaviour
{
    [SerializeField] private LayerMask byPass;
    [SerializeField] private BoxCollider boxCollider;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == byPass)
        {
            boxCollider.isTrigger = true;
        }
        else
        {
            boxCollider.isTrigger = false;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == byPass)
        {
            boxCollider.isTrigger = true;
        }
        else
        {
            boxCollider.isTrigger = false;
        }
    }
}
