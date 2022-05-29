using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CheckPoint : MonoBehaviour
{
    public LevelRespawnSystem respawnSystem;
    public int ID;
    public bool isActive = false;

    private void Start()
    {
        isActive = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isActive)
        {
            if (respawnSystem.playerRespawnPoint[respawnSystem.currentCheckpointID] == transform)
            {
                respawnSystem.UpdateCheckPoint(ID);
                isActive = true;
            }
        }
    }

}
