using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoLock : MonoBehaviour
{
    public static AutoLock autoLock;
    public List<GameObject> enemies;
    private Camera cam;
    [HideInInspector] public bool activated;
    public Bullet[] bullets;

    void Start()
    {
        autoLock = this;
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        bullets = FindObjectsOfType<Bullet>();

        if (Gun.gun.ReloadButtonPressed())
        {
            ShootRay();
            if (enemies.Count > 0)
            {
                for (int i = 0; i < enemies.Count; i++)
                {
                    bullets[i].AddTarget(enemies[0]);
                    enemies.RemoveAt(0);
                }
                activated = true;
            }
        }
    }

    private void ShootRay()
    {
        RaycastHit hit;
        Physics.Raycast(cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)), out hit);
        if (hit.collider != null)
        {
            if (hit.collider.GetComponent<EnemyController>() != null)
            {
                if (enemies.Count < bullets.Length && !enemies.Contains(hit.collider.gameObject)) enemies.Add(hit.collider.gameObject);
            }
        }
    }
}
