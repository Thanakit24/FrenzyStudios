using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StanceController : MonoBehaviour
{
    public KeyCode stanceChange;
    public KeyCode action;

    public FumaController shuriken;

    public bool throwingStanceActiveStatus()
    {
        if (Input.GetKey(stanceChange))
        {
            return true;
        }
        return false;
    }

    

    void Update()
    {
        shuriken.enabled = throwingStanceActiveStatus();

    }
}
