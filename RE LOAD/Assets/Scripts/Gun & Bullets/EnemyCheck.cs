using UnityEngine;

public class EnemyCheck : MonoBehaviour
{
    private int damage;

    private void Start()
    {
        //damage = gameObject.GetComponentInParent<Bullet>().damage;
        damage = 10;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.name);
        if (other.CompareTag("Enemy"))
        {
            Health enemy = other.GetComponent<Health>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}
