using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    public bool isDestroyed;
    [SerializeField] GameObject active, inactive;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Shuriken"))
        {
            isDestroyed = true;

            inactive.SetActive(true);
            active.SetActive(false);
        }
    }
}
