using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy2Controller : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private LayerMask whatIsPlayer;
    private Vector3 currentDestination;
    Rigidbody rb;
    public float moveSpeed;

    //[Header("Idle Config")]
    private Vector3 homePos;

    [Header("Attacking Config")]
    public float explosionDelay;
    public float explosionRadius, explosionForce, upwardsModifier;

    [Header("States")]
    [SerializeField] private float surroundSightRange;
    [SerializeField] private float forwardSightRange;
    public LayerMask obstructionMask;
    [Space(10)]
    [SerializeField] private float attackRange;
    [SerializeField] private bool playerInSight, playerInAttackRange, noticedPlayer;

    private void Start()
    {
        //player = PlayerController.instance.GetComponent<Transform>();
        rb = GetComponent<Rigidbody>();
        //agent = GetComponent<NavMeshAgent>();
        homePos = transform.position;
        noticedPlayer = false;
    }

    private void Update()
    {
        //Check for sight and attack range
        playerInSight = Physics.CheckSphere(transform.position, surroundSightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (Physics.Raycast(transform.position, player.position - transform.position, surroundSightRange, whatIsPlayer)) noticedPlayer = true;

        if (!noticedPlayer) Idle();
        else
        {
            if (playerInSight && !playerInAttackRange) ChasePlayer();
            if (playerInSight && playerInAttackRange) AttackPlayer();
        }
       

    }


    //=================================================================================

    private void Idle()
    {
        SetDestination(homePos);
        LookAt(homePos);
    }

    //=================================================================================

    private void ChasePlayer()
    {
        SetDestination(player.position);
        LookAt(player.position);
    }

    //=================================================================================

    private void AttackPlayer()
    {
        SetDestination(transform.position);
        //hand.transform.LookAt(player);
        //attackPoint.transform.LookAt(player);
        LookAt(player.position);
        //transform.rotation = Quaternion.Lerp()

        InitialSelfDestruct();
    }

    private void InitialSelfDestruct()
    {
        StartCoroutine(Wait(explosionDelay));

        Explode();
    }


    IEnumerator Wait(float x)
    {
        yield return new WaitForSeconds(x);
    }

    void Explode()
    {
        RaycastHit[] inRagedObjects = Physics.SphereCastAll(transform.position, explosionRadius, Vector3.zero);
        foreach (var item in inRagedObjects)
        {
            Rigidbody _rigidbody;
            if (item.collider.gameObject.TryGetComponent<Rigidbody>(out _rigidbody))
            {
                if (item.collider.CompareTag("Player"))
                {
                    item.collider.GetComponent<PlayerController>().Knocked(0);
                }
                _rigidbody.AddExplosionForce(explosionForce, transform.position, explosionRadius, upwardsModifier);
            }
        }

        Destroy(this.gameObject);
    }

    //=================================================================================
    private void OnDrawGizmosSelected()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, surroundSightRange);

        Gizmos.DrawWireSphere(transform.position, forwardSightRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    private void LookAt(Vector3 pos)
    {
        //transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(pos - transform.position, Vector3.up));
        transform.LookAt(pos);
    }

    void SetDestination(Vector3 pos)
    {
        currentDestination = pos;
    }

    void DestinationExecution()
    {
        Vector3 dir = (currentDestination- transform.position).normalized;
        rb.velocity = dir * moveSpeed;
    }
}
