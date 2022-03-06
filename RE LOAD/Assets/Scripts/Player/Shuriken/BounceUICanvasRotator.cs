using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceUICanvasRotator : MonoBehaviour
{
    float destroyTimer = 1;
    float counter = 0;

    void Update()
    {
        if (PlayerController.instance != null)
            transform.LookAt(PlayerController.instance.transform.position);

        if (counter < destroyTimer) counter += Time.deltaTime;
        else Destroy(gameObject);
    }
}
