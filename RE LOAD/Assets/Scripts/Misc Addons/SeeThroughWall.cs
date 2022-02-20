using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeeThroughWall : MonoBehaviour
{
    public Transform shuriken;
    public Material transparent;
    public GameObject col;
    public List<GameObject> objs;
    public List<Material> materials;

    public void Update()
    {
        float dist = Mathf.Abs(Vector3.Magnitude(transform.position - shuriken.position));
        Ray ray = new Ray(transform.position, shuriken.position - transform.position);

        //RaycastHit[] hits = Physics.RaycastAll(ray, dist);
        RaycastHit[] hits = Physics.RaycastAll(ray, dist);
        foreach (RaycastHit hit in hits)
        {
            if (!hit.collider.CompareTag("Player") || !hit.collider.CompareTag("Shuriken") || hit.collider.gameObject.name != "Col")
            {
                if (!objs.Contains(hit.collider.gameObject))
                {
                    objs.Add(hit.collider.gameObject);

                    MeshRenderer mr = hit.collider.GetComponent<MeshRenderer>();

                    if (mr != null)
                    {
                        materials.Add(mr.material);
                        mr.material = transparent;
                    }
                }
                
            }
        }


        if (hits.Length == 0 && objs.Count>0)
        {
            objs.Remove(col);
            for (int i = 0; i < objs.Count; i++)
            {
                objs[i].GetComponent<MeshRenderer>().material = materials[i];
            }

            objs.Clear();
            materials.Clear();
        }
    }
}
