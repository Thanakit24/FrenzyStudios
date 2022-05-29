using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthEditor : MonoBehaviour
{
    GameObject[] entityOBJ;
    public List<Health> entity;

    public PlayerController player;

    void Start()
    {
        foreach (var item in entityOBJ)
        {
            entity.Add(GameObject.FindGameObjectWithTag("Enemy").GetComponent<Health>());
        }
    }

    void Update()
    {
        if (player.isMeleeing)
        {

        }
    }
}
