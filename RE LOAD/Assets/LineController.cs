using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineController : MonoBehaviour
{
    public GameObject prefab;

    public List<GameObject> lines;

    public int maxBounces = 2;

    private void Update()
    {
        RayCast(transform.position, transform.forward);
    }

    void RayCast(Vector3 pos, Vector3 dir)
    {
        for (int i = 0; i < maxBounces; i++)
        {
            Ray ray = new Ray(pos, dir);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Vector3 current = pos;
                pos = hit.point;
                dir = Vector3.Reflect(dir, hit.normal);

                if (lines.Count < maxBounces)
                {
                    CreateLine(current, pos);
                }
                else
                {
                    RepositionLine(lines[0], current, pos);
                }
            }
            else
            {
                break;
            }
        }
    }

    void CreateLine(Vector3 start, Vector3 end)
    {
        GameObject newLine = Instantiate(prefab, start, Quaternion.identity);
        LineRenderer newLineRender = newLine.GetComponent<LineRenderer>();

        newLineRender.SetPosition(0,start);
        newLineRender.SetPosition(1,end);

        lines.Add(newLine);
    }

    void RepositionLine(GameObject line, Vector3 start, Vector3 end)
    {
        LineRenderer lr = line.GetComponent<LineRenderer>();
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
    }
}
