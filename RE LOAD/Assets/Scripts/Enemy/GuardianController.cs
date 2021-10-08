using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuardianController : MonoBehaviour
{
    public NavMeshAgent agent;
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask whatIsPlayer;
    [SerializeField] private Feet feet;
    //[SerializeField] private GameObject hand;

    [SerializeField] private GameObject shield;

    [Header("Patrolling")]
    public bool isStationary;
    public Vector3[] targetWalkPoints;
    [SerializeField] private bool TwoDirectionPath;
    private bool isReturning;
    private int walkPointIndex;
    [SerializeField] private Vector3 walkPoint;

    [Header("Attacking")]
    [SerializeField] private float attackRecoveryTime;
    [SerializeField] private float attackAnticipationTime;
    [SerializeField] private bool hasAttacked;
    [SerializeField] private Transform baton;
    private Quaternion batonOriginalPos;
    //[SerializeField] private float hitSpeed;
    [SerializeField] private int batonDamage;

    [Header("States")]
    [SerializeField] private float sightRange;
    [SerializeField] private float attackRange;
    [SerializeField] private bool playerInSightRange, playerInAttackRange;

    public Vector3 distanceToWalkPoint;

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
        batonOriginalPos = baton.localRotation;
    }

    private void Update()
    {
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);


        if (!playerInSightRange && !playerInAttackRange && !isStationary) Patroling();
        if (playerInSightRange && !playerInAttackRange && !hasAttacked) ChasePlayer();
        if (playerInSightRange && playerInAttackRange && !hasAttacked) Invoke(nameof(AttackPlayer), attackAnticipationTime);
        if (hasAttacked) Invoke(nameof(ResetAttack), attackRecoveryTime);

    }

    //=================================================================================
    //================================= GUARDIAN ======================================
    //=================================================================================

    private void Patroling()
    {
        agent.SetDestination(walkPoint);
        distanceToWalkPoint = transform.position - walkPoint;


        //float y = transform.position.y;
        float y;
        if (walkPointIndex > 1) y =  transform.position.y - (targetWalkPoints[walkPointIndex].y - targetWalkPoints[walkPointIndex - 1].y);
        else y = transform.position.y;

        if (distanceToWalkPoint.magnitude > 2f) transform.LookAt(new Vector3(walkPoint.x, y, walkPoint.z));

        if (distanceToWalkPoint.magnitude < 0.2f)
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
                if (walkPointIndex < targetWalkPoints.Length - 1) walkPointIndex++;
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
    //================================= GUARDIAN ======================================
    //=================================================================================

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
        transform.LookAt(new Vector3(player.position.x, transform.position.y, player.position.z));
    }

    //=================================================================================
    //================================= GUARDIAN ======================================
    //=================================================================================

    private void AttackPlayer()
    {
        agent.SetDestination(transform.position);
        //transform.LookAt(player.position, Vector3.forward);

        //transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(player.position - transform.position, Vector3.up));

        batonAttack(true);

        hasAttacked = true;
    }

    private void batonAttack(bool attack)
    {
        if (attack)
        {
            //Debug.Log("Attacking");
            baton.localRotation = Quaternion.AngleAxis(90, Vector3.right);
        }
        else baton.localRotation = batonOriginalPos;
    }

    private void ResetAttack()
    {
        batonAttack(false);
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

