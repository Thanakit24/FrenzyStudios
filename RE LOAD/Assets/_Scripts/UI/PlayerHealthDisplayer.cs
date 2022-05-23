using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using MoreMountains.NiceVibrations;

public class PlayerHealthDisplayer : MonoBehaviour
{
    public MMProgressBar healthBar;
    public PlayerController player;
    void Start()
    {
        player = PlayerController.instance;

        healthBar.EndValue = player.health;
    }

    void Update()
    {
        
    }
}
