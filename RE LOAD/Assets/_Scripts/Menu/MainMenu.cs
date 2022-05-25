using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    public void Play()
    {
        SceneManager.LoadScene("Level3Lighting");
    }

    // Update is called once per frame
    public void LevelSelection()
    {
        SceneManager.LoadScene("LevelSelection");
    }

    public void Setting()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Quit()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }

}
