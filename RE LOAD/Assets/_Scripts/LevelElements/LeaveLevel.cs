using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaveLevel : MonoBehaviour
{
    public int nextScene;

    private void OnTriggerEnter(Collider other)
    {
        GameCanvasController.instance.LoadLevel(nextScene);
    }
}
