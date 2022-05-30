using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using MoreMountains.Feedbacks;

public class LevelRespawnSystem : MonoBehaviour
{
    public static LevelRespawnSystem instance;

    public TextMeshProUGUI timerText;

    public PlayerController player;

    public List<Transform> playerRespawnPoint;
    public List<MMFeedbacks> explosionList;
    public List<float> timer;
    public float timerCounter;

    public int currentCheckpointID;
    public bool shouldPlay;

    public MMFeedbacks timerSound;
    private void Start()
    {
        //player = PlayerController.instance;
        //player.health = 0;
        currentCheckpointID = 0;
        timerCounter = timer[0];

        shouldPlay = false;
    }

    private void Update()
    {
        if (timerCounter > 0)
        {
            timerCounter -= Time.deltaTime;

            if (timerCounter <= 8.9f && timerCounter > 8.5f)
            {
                timerSound.PlayFeedbacks();
            }
        }
        else
        {
            timerSound.StopAllCoroutines();
            timerSound.StopFeedbacks();
            PlayExplosion();
            player.health = 0;
        }

        timerText.text = timerCounter.ToString();

        //if (player = null) player = PlayerController.instance;
    }

    IEnumerator DelayedRestart()
    {
        yield return new WaitForSeconds(1f);
        RespawnPlayerAtCheckPoint();
    }

    public void RespawnPlayerAtCheckPoint()
    {
        player.health = player.maxHealth;
        player.transform.position = GetRespawnPoint();
        player.transform.localRotation = GetRotation();
        GameCanvasController.instance.currentState = GameState.gameplay;

        SetTimer();
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
        PlayExplosion();
        currentCheckpointID = ID;
        SetTimer();
    }

    void SetTimer()
    {
        timerCounter = timer[0];
        timerSound.StopFeedbacks();
        timerSound.StopAllCoroutines();
    }
    public void PlayExplosion()
    {
        if (!shouldPlay)
        {
            shouldPlay = true;
        }
        else
        {
            explosionList[0].PlayFeedbacks();
        }
    }
}
