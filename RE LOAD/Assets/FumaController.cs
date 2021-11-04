using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FumaState
{
    InHands,
    Flying,
    Returning,
    Ragdoll,
    Stuck
}

public class FumaController : MonoBehaviour
{
    public FumaState state = FumaState.InHands;
    public Transform player, holder;


    public Vector3 curveRot, throwRotation;
    public float flyingSpeed, chargeSpeed, maxBounces, ySpinSpeed, xSpinSpeed, pickupRange, destroyDistance, fxDestroyTime, ragdollSpin;
    public bool curvedStart, curvedFlying, curvedReturn;

    public MeshCollider col;
    Vector3 lastPos, returnPos;
    Rigidbody rb;
    Transform model, cam;
    public GameObject impactFX, trailFX;
    public bool mustReturn = false, firstBounce;
    float tempBounces;
    int bounces;


    void Start()
    {
        cam = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
