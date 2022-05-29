using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelRespawnSystem : MonoBehaviour
{
    public static LevelRespawnSystem instance;


    public PlayerController player;

    public List<Transform> playerRespawnPoint;
    public List<ParticleSystem> explosionList;
    public List<float> timer;
    public float timerCounter;

    public int currentCheckpointID;

    private void Start()
    {
        player = PlayerController.instance;
        //player.health = 0;
        currentCheckpointID = 0;
    }

    private void Update()
    {
        if (timerCounter > 0)
        {
            timerCounter -= Time.deltaTime;
        }
        else
        {
            player.health = 0;
        }
    }

    public void RespawnPlayerAtCheckPoint()
    {
        player.health = player.maxHealth;
        player.transform.position = GetRespawnPoint();
        player.transform.localRotation = GetRotation();
        GameCanvasController.instance.currentState = GameState.gameplay;

        timerCounter = timer[currentCheckpointID-1];
    }

    public Vector3 GetRespawnPoint()
    {
        return playerRespawnPoint[currentCheckpointID-1].position;
    }

    public Quaternion GetRotation()
    {
        return playerRespawnPoint[currentCheckpointID - 1].rotation;
    }

    public void UpdateCheckPoint(int ID)
    {
        currentCheckpointID = ID;
        timerCounter = timer[currentCheckpointID - 1];
    }
}
