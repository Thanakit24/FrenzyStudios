using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class KnockbackSFXEnabler : MonoBehaviour
{
    public Rigidbody rb;
    public MMFeedbacks hitSFX;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag("Player"))
        {
            hitSFX.PlayFeedbacks();
        }
    }
}
