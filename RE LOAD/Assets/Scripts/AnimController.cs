using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimController : MonoBehaviour
{
    public Animator anim;


    void Start()
    {
        anim = GetComponent<Animator>();

    }

    public void MeleeAttack()
    {
        PlayerController.instance.Melee();
    }
}
