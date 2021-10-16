using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    public NavMeshAgent agent;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private Feet feet;
    [SerializeField] private GameObject hand;


    [Header("Patrolling")]
    public bool isStationary;
    public Vector3[] targetWalkPoints;
    [SerializeField] private bool TwoDirectionPath; //Dont check this button if you want the enemy to loop 1-2-3-4-3-2-1-2-3-4. Tick this if you want it to go 1-2-3-4-1-2-3-4
    private bool isReturning;
    private int walkPointIndex;
    private Vector3 walkPoint;
    private bool walkPointSet;

    [Header("Attacking")]
    [SerializeField] private float timeBetweenAttacks;
    private bool hasAttacked;
    public GameObject enemyBullet;
    [SerializeField] private Transform attackPoint;

    [Header("States")]
    [SerializeField] private float sightRange;
    [SerializeField] private float attackRange;
    [SerializeField] private bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        feet = GetComponentInChildren<Feet>();
        agent = GetComponent<NavMeshAgent>();
        walkPointIndex = 0;

        if (!isStationary)
        {
            transform.LookAt(targetWalkPoints[walkPointIndex]);
            isReturning = false;
            walkPoint = targetWalkPoints[walkPointIndex];
        }
    }

    private void Update()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange && !isStationary) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInSightRange && playerInAttackRange) AttackPlayer();

    }
    //=================================================================================
    private void Patroling()
    {
        agent.SetDestination(walkPoint);
        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 0.1f)
        {
            SetNextWalkPoint();
            walkPoint = targetWalkPoints[walkPointIndex];
        }
    }

    private void SetNextWalkPoint()
    {
        if (!TwoDirectionPath)
        {
            if (!isReturning)
            {
                if (walkPointIndex < targetWalkPoints.Length-1) walkPointIndex++;
                else isReturning = true;
            }
            if (isReturning)
            {
                if (walkPointIndex > 0) walkPointIndex--;
                else
                {
                    isReturning = false;
                }
                if (walkPointIndex < 0) walkPointIndex = 1;
            }
        }
        else
        {
            walkPointIndex++;
            if (walkPointIndex == targetWalkPoints.Length) walkPointIndex = 0;
        }
    }


    //=================================================================================

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
        //Debug.Log("chasingPlayer");
    }

    //=================================================================================

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);
        hand.transform.LookAt(player);
        attackPoint.transform.LookAt(player);
        transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(player.position - transform.position, Vector3.up));
        //transform.rotation = Quaternion.Lerp()

        if (!hasAttacked)
        {
            Rigidbody rb = Instantiate(enemyBullet, attackPoint.position, attackPoint.rotation).GetComponent<Rigidbody>();
            rb.AddForce(attackPoint.transform.forward * 25f, ForceMode.Impulse);

            hasAttacked = true;
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }

    }

    private void ResetAttack()
    {
        hasAttacked = false;
    }

    //=================================================================================
    private void OnDrawGizmosSelected()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
