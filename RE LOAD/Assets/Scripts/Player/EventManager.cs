using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager : MonoBehaviour
{
    public static event Action Teleport;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            Teleport?.Invoke();
    }
}
