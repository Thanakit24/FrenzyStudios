using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameState { gameplay, paused, dies, loading }

public class GameCanvasController : MonoBehaviour
{
    public static GameCanvasController instance;
    public LevelRespawnSystem levelRespawnSystem;


    public GameState currentState;
    private GameState lastState;
    private float pausedTimescale = 0;
    private float lastTimescale;

    PlayerController player;

    [Header("First Generation")]
    public GameObject HUD;
    public GameObject pauseMenu;
    public GameObject dieScreen;
    public GameObject fader;
    public GameObject skillTree;

    [Header("HUD")]
    public float health;
    public float dashCooldown;
    public float teleportCooldown;
    public bool electrolyzedShuriken;
    public Slider healthDisplayer;
    public bool skillTreeActive;

    [Header("Fader Config")]
    public Animator faderAnim;
    public float transitionTime;

    //[Header("Bounce UI Indicator Config")]
    //public GameObject[] prefabs;
    //public GameObject[] current;

    void Awake()
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("GameCanvas");

        if (objs.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);

        faderAnim.Play("Start");
        /*
        skillTree.SetActive(false);
        skillTreeActive = false;
        */
        instance = this;
        player = PlayerController.instance;

        UpdateCanvasState(GameState.gameplay);
        dieScreen.SetActive(false);
    }


    // Update is called once per frame
    void Update()
    {
        #region Pausing System
        if (!currentState.Equals(GameState.loading) || !currentState.Equals(GameState.dies))
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

        if (player == null) player = PlayerController.instance;
        healthDisplayer.value = player.health/player.maxHealth;

        pauseMenu.SetActive(currentState.Equals(GameState.paused));

        if (player.health <= 0)
            UpdateCanvasState(GameState.dies);
        else dieScreen.SetActive(false);

        if (Input.GetKeyDown(KeyCode.I)) LoadLevel(0);

        if (currentState.Equals(GameState.dies))
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                levelRespawnSystem = player.levelRespawnSystem;
                if (levelRespawnSystem != null)
                {
                    levelRespawnSystem.RespawnPlayerAtCheckPoint();
                }
                else
                {
                    LoadLevel(SceneManager.GetActiveScene().buildIndex);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.F11))
        {
            if (SceneManager.GetActiveScene().buildIndex > 0) LoadLevel(SceneManager.GetActiveScene().buildIndex - 1);
        }
        else if (Input.GetKeyDown(KeyCode.F12))
        {
            if (SceneManager.GetActiveScene().buildIndex < 5) LoadLevel(SceneManager.GetActiveScene().buildIndex + 1);
        }
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
                //Cursor.visible = false;
                Time.timeScale = 1;

                break;


            case GameState.paused:
                if (currentState.Equals(GameState.paused))
                {
                    //Cursor.visible = true;
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
                dieScreen.SetActive(true);
                
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

    public void LoadLevel(int sceneNumber)
    {
        StartCoroutine(LoadLevelEnum(sceneNumber));
    }

    IEnumerator LoadLevelEnum(int sceneNumber)
    {
        faderAnim.Play("NextScene");

        yield return new WaitForSeconds(transitionTime);

        SceneManager.LoadScene(sceneNumber);
        faderAnim.Play("Start");
    }
}
