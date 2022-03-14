using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamReposition : MonoBehaviour
{
    Transform parent;
    Vector3 localPos;
    void Start()
    {
        parent = transform.parent;
        localPos = transform.localPosition;
        transform.parent = null;
    }

    void Update()
    {
        transform.position = parent.position + localPos + Vector3.up * 0.01f;
        transform.rotation = parent.rotation;
    }
}
