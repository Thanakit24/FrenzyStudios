using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [SerializeField] GameObject door;
    [SerializeField] bool shootable;

    bool isOpened = false;
    Renderer rend;

    private void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void OnTriggerStay(Collider col)
    {
        if(col.gameObject.tag == "Player" && shootable == false)
        {
            Open();
        }

        if(col.gameObject.tag == "Bullet" && shootable == true)
        {
            Open();
        }
    }

    void Open()
    {
        door.transform.position += new Vector3(0, 4, 0);

        this.gameObject.transform.position -= new Vector3(0, 0.2f, 0);
        rend.material.SetColor("_Color", Color.green);
    }
}
