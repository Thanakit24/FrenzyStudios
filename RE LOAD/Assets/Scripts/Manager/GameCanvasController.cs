using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCanvasController : MonoBehaviour
{
    public static GameCanvasController instance;

    [Header("Data")]
    public float health;
    public float dashCooldown;
    public float teleportCooldown;
    public bool electrolyzedShuriken;

    [Header("Display")]
    public GameObject healthDisplayer;

    [Header("Bounce UI Indicator Config")]
    public GameObject[] prefabs;
    public GameObject[] current;

    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SpawnBounceUIIndicator(Vector3 worldPos)
    {
        //float dist = Vector3.Distance(PlayerController.instance.transform.position, worldPos);
        //Vector2 spawnPos = Camera.main.WorldToViewportPoint

    }
}
