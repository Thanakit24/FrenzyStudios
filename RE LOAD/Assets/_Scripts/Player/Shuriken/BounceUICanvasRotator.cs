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
        transform.LookAt(Camera.main.transform.position);

        if (!onlyDestroyOnActivated)
        {

            if (counter < destroyTimer) counter += Time.deltaTime;
            else Delete();
        }
    }

    public void Delete()
    {
        Destroy(this.gameObject);
    }
}
