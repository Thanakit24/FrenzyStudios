using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaveLevel : MonoBehaviour
{
    public int nextScene;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) GameCanvasController.instance.LoadLevel(nextScene);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
