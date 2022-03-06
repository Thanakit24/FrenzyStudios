using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceUICanvasRotator : MonoBehaviour
{
    float destroyTimer = 1;
    float counter = 0;

    public bool onlyDestroyOnActivated;

    void Update()
    {
        transform.LookAt(PlayerController.instance.transform.position);

        if (!onlyDestroyOnActivated)
        {

            if (counter < destroyTimer) counter += Time.deltaTime;
            else Destroy(gameObject);
        }
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
    }
}
