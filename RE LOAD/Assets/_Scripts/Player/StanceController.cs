using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class StanceController : MonoBehaviour
{
    public KeyCode stanceChange;
    public KeyCode action;

    public FumaController shuriken;
    public PlayerController player;
    public Animator animator;

    public float teleportSlowedTime;

    [Header("Feedbacks")]
    public MMFeedbacks ThrowingStance_Start;
    public MMFeedbacks ThrowingStance_Reset;
    public float _slowedTime;
    public float delayZoom;
    private float zoomCounter;

    private void Start()
    {
        player = gameObject.GetComponent<PlayerController>() ;
    }

    public bool throwingStanceActiveStatus()
    {
        if (Input.GetKeyDown(stanceChange))
        {
            if (shuriken.state.Equals(FumaState.InHands))
            {
                ThrowingStance_Start.PlayFeedbacks();

                if (!player.feet.isGrounded)
                    Time.timeScale = _slowedTime;
            }
            else if (shuriken.state.Equals(FumaState.Flying) || shuriken.state.Equals(FumaState.Stuck) || shuriken.state.Equals(FumaState.Ragdoll))
            {
                shuriken.shouldLockOnToPlayer = true;
            }

            return true;
        }
        else if (Input.GetKey(stanceChange))
        {
            if (shuriken.state.Equals(FumaState.InHands))
                shuriken.RepositionLine(Camera.main.transform.position, Camera.main.transform.forward, false);

            if (Input.GetKeyDown(action))
                shurikenThrow();

            return true;
        }
        else if (Input.GetKeyUp(stanceChange) && shuriken.state.Equals(FumaState.InHands))
        {
            shurikenThrow();
            return false;
        }

        ThrowingStance_Reset.PlayFeedbacks();
        return false;
    }

    void Update()
    {
        if (player.isTeleporting)
        {
            Time.timeScale = teleportSlowedTime;
            return;
        }

        if (!throwingStanceActiveStatus())
        { 
            Time.timeScale = 1;

            animator.enabled = true;
        }

        

        if (!shuriken.state.Equals(FumaState.InHands))
        {
            animator.enabled = false;
        }
    }

    void shurikenThrow()
    {
        animator.enabled = false;
        shuriken.Throw();
    }
}
