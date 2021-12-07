using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    public GameObject prefab;

    void RayCast(Vector3 pos, Vector3 dir)
    {
        for (int i = 0; i < 2; i++)
        {
            Ray ray = new Ray(pos, dir);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 10))
            {
                Debug.DrawLine(pos, hit.point, Color.red);

                pos = hit.point;
                dir = Vector3.Reflect(dir, hit.normal);
            }
            else
            {
                Debug.DrawRay(pos, dir * 10, Color.blue);
                break;
            }
        }
    }

    void Create(Vector3 start, Vector3 end)
    {
        GameObject newLine = Instantiate(prefab, start, Quaternion.identity);
        LineRenderer newLineRender = newLine.GetComponent<LineRenderer>();

        newLineRender.SetPosition(0,start);
        newLineRender.SetPosition(1,end);
    }
}
