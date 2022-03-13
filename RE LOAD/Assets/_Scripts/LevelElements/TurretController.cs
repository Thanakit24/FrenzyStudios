using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask whatIsPlayer;

    [Header("Attacking")]
    [SerializeField] private float timeBetweenAttacks;
    private bool hasAttacked;
    public GameObject enemyBullet;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private int maxBulletFired;
    [SerializeField] private int bulletFired;
    [SerializeField] private float reloadSpeed;
    private bool reloading;
    

    [Header("States")]
    [SerializeField] private float attackRange;
    [SerializeField] private bool playerInAttackRange;

    [Header("Parts")]
    [SerializeField] private GameObject turretBody;
    [SerializeField] private GameObject turretBarrel;

    private void Update()
    {
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);
        if (playerInAttackRange) AttackPlayer();
    }

    private void AttackPlayer()
    {
        turretBody.transform.LookAt(player);
        turretBarrel.transform.LookAt(player);
        attackPoint.transform.LookAt(player);
        transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(player.position - transform.position, Vector3.up));

        if (!hasAttacked && !reloading)
        {
            Rigidbody rb = Instantiate(enemyBullet, attackPoint.position, attackPoint.rotation).GetComponent<Rigidbody>();
            rb.AddForce(attackPoint.transform.forward * 25f, ForceMode.Impulse);
            bulletFired = bulletFired + 1;

            hasAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }

        if (bulletFired == maxBulletFired)
		{
            reloading = true;
            StartCoroutine(Reload());
		}
    }

    private void ResetAttack()
    {
        hasAttacked = false;
    }

    IEnumerator Reload()
    {
        bulletFired = 0;
        yield return new WaitForSeconds(reloadSpeed);
        reloading = false;
    }

    //=================================================================================
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
