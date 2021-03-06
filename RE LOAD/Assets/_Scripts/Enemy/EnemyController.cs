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
    [SerializeField] private Animator animator;
    [SerializeField] private Vector3 center;

    [Header("Patrolling")]
    public bool isStationary;
    public Vector3[] targetWalkPoints;
    [SerializeField] private bool TwoDirectionPath; //Dont check this button if you want the enemy to loop 1-2-3-4-3-2-1-2-3-4. Tick this if you want it to go 1-2-3-4-1-2-3-4
    private bool isReturning;
    private int walkPointIndex;
    private Vector3 walkPoint;
    private bool walkPointSet;
    public float rememberCountdown;
    public float rememberTime;
    public bool remembers;
    public float confusedTime;

    [Header("Attacking")]
    [SerializeField] private float timeBetweenAttacks;
    private float timeBetweenAttacksCounter =0;
    private bool hasAttacked;
    private Vector3 attackPointOffset;
    public GameObject enemyBullet;
    [SerializeField] private Transform attackPoint;

    [Header("States")]
    [SerializeField] private float surroundSightRange;
    [SerializeField] private float forwardSightRange;
    [SerializeField] private float sightAngle = 150f;
    public LayerMask obstructionMask;
    [Space(10)]
    [SerializeField] private float attackRange;
    [SerializeField] private bool playerInSight, playerInAttackRange;

    private float DistToPlayer()
    {
        return (player.position - transform.position).magnitude;
    }

    private bool InLosOfPlayer()
    {
        if (Physics.CheckSphere(transform.position, forwardSightRange, whatIsPlayer))
        {
            Vector3 dir = (player.position - transform.position).normalized;
            float angle = Vector3.Angle(transform.forward, dir);

            if (angle < sightAngle)
            {
                float dist = Vector3.Distance(transform.position, player.position);

                if (!Physics.Raycast(transform.position, dir, dist, obstructionMask))
                {
                    rememberCountdown = rememberTime;
                    return true;
                }
                else return false;
            }
            else return false;
        }
        else
        {
            return false;
        }
    }

    private void Start()
    {
        player = PlayerController.instance.GetComponent<Transform>();
        feet = GetComponentInChildren<Feet>();
        walkPointIndex = 0;

        if (!isStationary && targetWalkPoints.Length > 0)
        {
            //transform.LookAt(targetWalkPoints[walkPointIndex]);
            isReturning = false;
            walkPoint = targetWalkPoints[walkPointIndex];
        }

        attackPointOffset = attackPoint.localRotation.eulerAngles;
    }

    private void Update()
    {
        //Check for sight and attack range
        playerInSight = InLosOfPlayer();
        if (Physics.CheckSphere(transform.position, surroundSightRange, whatIsPlayer)) playerInSight = true;
        if (rememberCountdown > 0)
        {
            remembers = true;
            rememberCountdown -= Time.deltaTime;
        }
        else
        {
            timeBetweenAttacksCounter = timeBetweenAttacks;
            remembers = false;
        }
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        
        if (!playerInSight && !playerInAttackRange && !isStationary && !remembers) Patroling();
        if (playerInSight && !playerInAttackRange) ChasePlayer();
        if (playerInSight && playerInAttackRange) AttackPlayer();
        else animator.SetBool("isFiring", false);


        if (remembers && !playerInSight && !playerInAttackRange) Confused();

        animator.SetBool("Walking", agent.velocity.normalized.magnitude > 0.5f);

        if (player.GetComponent<PlayerController>().isMeleeing && DistToPlayer() <= 2f)
        {
            Destroy(gameObject);
        }
    }


    //=================================================================================
    private void Patroling()
    {
        if ((walkPoint -transform.position).magnitude > 1) LookAt(walkPoint);
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


    private void Confused()
    {
        agent.Stop();
        StartCoroutine(Wait());
    }

    //=================================================================================

    private void ChasePlayer()
    {
        agent.SetDestination(player.position);
        if (DistToPlayer() > 5 ) LookAt(player.position - player.right *0.3f);
        else LookAt(player.position);


        //Debug.Log("chasingPlayer");
    }

    //=================================================================================

    private void AttackPlayer()
    {
        if (!agent.isStopped) agent.isStopped = true;
        
        //if (timeBetweenAttacksCounter <= 0) animator.SetBool("isFiring", true);
        //else timeBetweenAttacksCounter -= Time.deltaTime;
        StartCoroutine(ShootAndDelay());
    }

    public IEnumerator ShootAndDelay()
    {
        animator.SetBool("isFiring", true);
        yield return new WaitForSeconds(timeBetweenAttacks);

        animator.SetBool("isFiring", false);
        if (DistToPlayer() > 5) LookAt(player.position - player.right * 0.3f);
        else LookAt(player.position);

    }

    public void Shoot()
    {
        Rigidbody rb = Instantiate(enemyBullet, attackPoint.position, attackPoint.rotation).GetComponent<Rigidbody>();
        rb.AddForce(attackPoint.forward * 25f, ForceMode.Impulse);
        if (DistToPlayer() > 5) LookAt(player.position - player.right * 0.3f);
        else LookAt(player.position);
    }
    public void ResetShooting()
    {
        agent.updateRotation = true;
        if (DistToPlayer() > 5) LookAt(player.position - player.right * 0.3f);
        else LookAt(player.position);
        //timeBetweenAttacksCounter = timeBetweenAttacks;
    }

    //=================================================================================
    private void OnDrawGizmosSelected()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position+ center, surroundSightRange);

        if (InLosOfPlayer())
        {
            Gizmos.DrawLine(transform.position + center, player.position);
            Gizmos.DrawWireSphere(transform.position+ center, forwardSightRange);

            Gizmos.color = Color.green;
            Gizmos.DrawRay(attackPoint.position, attackPoint.forward);
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + center, attackRange);

        
    }

    private void LookAt(Vector3 pos)
    {
        transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(pos - transform.position, Vector3.up));
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(confusedTime);
    }
    
}
