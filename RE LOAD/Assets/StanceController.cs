using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class StanceController : MonoBehaviour
{
    public KeyCode stanceChange;
    public KeyCode action;

    public FumaController shuriken;

    [Header("Feedbacks")]
    public MMFeedbacks ThrowingStance_Start;
    public MMFeedbacks ThrowingStance_Reset;

    public bool throwingStanceActiveStatus()
    {
        if (Input.GetKey(stanceChange))
        {
            if (shuriken.state.Equals(FumaState.InHands))
            {
                shuriken.RepositionLine(Camera.main.transform.position, Camera.main.transform.forward, false);
            }
            else if (shuriken.state.Equals(FumaState.Flying) || shuriken.state.Equals(FumaState.Stuck) || shuriken.state.Equals(FumaState.Ragdoll))
            {
                shuriken.shouldLockOnToPlayer = true;
            }
            ThrowingStance_Start.PlayFeedbacks();

            return true;
        }

        ThrowingStance_Reset.PlayFeedbacks();
        return false;
    }

    void Update()
    {
        //shuriken.canShoot = throwingStanceActiveStatus();

        if (Input.GetKeyUp(stanceChange) && shuriken.state.Equals(FumaState.InHands))
        {
            shuriken.Throw();
        }

        if (Input.GetKeyDown(action) && (shuriken.stance.Equals(FumaState.Flying) || shuriken.stance.Equals(FumaState.Flying)))
        {
            //Teleport and Melee
        }


        if (throwingStanceActiveStatus())
        {
            Time.timeScale = 0.2f;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
}
