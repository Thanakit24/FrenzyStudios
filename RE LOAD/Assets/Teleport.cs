using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public PlayerController player;

    public void TeleportSkill()
    {
        player.TeleportTo();
    }
}
