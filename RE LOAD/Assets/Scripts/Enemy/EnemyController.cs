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

    [Header("Patrolling")]
    //[SerializeField] private float walkPointRange;
    public Transform[] targetWalkPoints;
    [SerializeField] private bool loop;
    [SerializeField] private bool autoSearchWalkPoints;
    private bool isReturning;
    [SerializeField] private int walkPointIndex;
    [SerializeField] private Vector3 walkPoint;
    private bool walkPointSet;

    [Header("Attacking")]
    [SerializeField] private float timeBetweenAttacks;
    private bool hasAttacked;

    [Header("States")]
    [SerializeField] private float sightRange;
    [SerializeField] private float attackRange;
    [SerializeField] private bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        //player = GameObject.Find("Player").transform;
        feet = GetComponentInChildren<Feet>();
        agent = GetComponent<NavMeshAgent>();
        walkPointIndex = 0;
        /*
        if (autoSearchWalkPoints)
        {
            for (int i = 1; i < transform.childCount; i++) //notcounting the feet
            {
                targetWalkPoints[i] = transform.GetChild(i);
            }
        }
        */
        transform.LookAt(targetWalkPoints[walkPointIndex].position);
        isReturning = false;
        walkPoint = targetWalkPoints[walkPointIndex].position;

    }

    private void Update()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        //if (playerInSightRange && playerInAttackRange) AttackPlayer();

    }
    //=================================================================================
    private void Patroling()
    {
        agent.SetDestination(walkPoint);
        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        if (distanceToWalkPoint.magnitude < 0.1f)
        {
            SetNextWalkPoint();
            walkPoint = targetWalkPoints[walkPointIndex].position;
        }
    }

    private void SetNextWalkPoint()
    {
        if (!loop)
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
        Debug.Log("chasingPlayer");
    }

    //=================================================================================

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);
        transform.LookAt(player);

        if (!hasAttacked)
        {
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

        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.position, sightRange);
    }
}
