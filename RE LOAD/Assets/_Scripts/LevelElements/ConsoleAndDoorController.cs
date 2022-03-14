using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConsoleAndDoorController : MonoBehaviour
{
    public GameObject door;
    bool playerIsInside;

    void Update()
    {
        if (playerIsInside && Input.GetKeyDown(KeyCode.F))
        {
            door.SetActive(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsInside = false;
        }
    }
}