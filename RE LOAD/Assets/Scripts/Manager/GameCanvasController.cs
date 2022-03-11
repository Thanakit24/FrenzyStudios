using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { gameplay, paused, dies, loading }

public class GameCanvasController : MonoBehaviour
{
    public static GameCanvasController instance;

    public GameState currentState;
    private GameState lastState;
    private float pausedTimescale = 0;
    private float lastTimescale;

    PlayerController player;

    [Header("First Generation")]
    public GameObject HUD;
    public GameObject pauseMenu;
    public GameObject fader;

    [Header("Data")]
    public float health;
    public float dashCooldown;
    public float teleportCooldown;
    public bool electrolyzedShuriken;

    [Header("Display")]
    public GameObject healthDisplayer;

    //[Header("Bounce UI Indicator Config")]
    //public GameObject[] prefabs;
    //public GameObject[] current;

    void Awake()
    {
        instance = this;

        UpdateCanvasState(GameState.gameplay);
        

    }


    // Update is called once per frame
    void Update()
    {
        #region Pausing System
        if (!currentState.Equals(GameState.loading))
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                UpdateCanvasState(GameState.paused);
            }
        }

        if (currentState.Equals(GameState.paused))
        {
            lastTimescale = Time.timeScale;
            Time.timeScale = 0;
        }
        #endregion

        pauseMenu.SetActive(currentState.Equals(GameState.paused));
        //fader.SetActive(currentState.Equals(GameState.loading));
    }

    void UpdateCanvasState(GameState newState)
    {
        GameState input = newState;

        if (currentState.Equals(GameState.paused) && newState.Equals(GameState.paused))
        {
            input = lastState;

            Time.timeScale = lastTimescale;
        }
        else input = newState;

        switch (input)
        {
            case GameState.gameplay:
                Cursor.visible = false;
                Time.timeScale = 1;

                break;


            case GameState.paused:
                if (currentState.Equals(GameState.paused))
                {
                    Cursor.visible = false;
                    lastTimescale = Time.timeScale;
                    Time.timeScale = pausedTimescale;

                    return;
                    PlayerController.instance.PlayerMovementInput = Vector3.zero;
                    PlayerController.instance.rawPlayerMovementInput = Vector3.zero;
                    PlayerController.instance.smartPlayerMovementInput = Vector3.zero;
                    PlayerController.instance.rb.isKinematic = true;
                }
                break;


            case GameState.dies:

                break;


            case GameState.loading:

                break;
        }

        if (!currentState.Equals(GameState.paused)) lastState = currentState;
        currentState = input;
    }

    public void SpawnBounceUIIndicator(Vector3 worldPos)
    {
        //float dist = Vector3.Distance(PlayerController.instance.transform.position, worldPos);
        //Vector2 spawnPos = Camera.main.WorldToViewportPoint

    }
}
