using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Watch : MonoBehaviour
{

    public TextMeshProUGUI ui;


    void Update()
    {
        ui.text = Time.realtimeSinceStartup.ToString() + "\n" + Time.timeScale.ToString() + "\n" + PlayerController.instance.isTeleporting.ToString() ;
    }
}
