using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMover : Target
{
    [Header("Agent Settings")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform[] patrollpoints;
    private int destPoint = 0;
    [SerializeField] private float patrolPointRadius = 0.5f; // Radius to consider reaching a patrol point
    [SerializeField] private bool doPatrolPause; // Option to pause at patrol points
    [SerializeField] private float patrolPauseDuration = 2f; // Duration of the pause at patrol points
    private bool doPause;
    Coroutine patrolPauseCoroutine;
    [SerializeField] private Transform chaseTarget;
    public Transform ChaseTarget { 
        get { return chaseTarget; } 
        set { chaseTarget = value; }
    }
    [SerializeField] private float minDistanceToTarget = 10f; // Distance at which the enemy will start chasing the target
    public float MinDistanceToTarget
    {
        get { return minDistanceToTarget; }
        set { minDistanceToTarget = value; }
    }
    private float distanceToTarget => chaseTarget ? Vector3.Distance(transform.position, chaseTarget.position) : Mathf.Infinity;
    public float DistanceToTarget => distanceToTarget;
    [SerializeField] Collider col;

    [Header("Animation Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationClip[] DeathAnimations; // Array of death animations to choose from


    private void Start()
    {
        agent.autoBraking = false;
        doPause = false;
        GotoNextPoint();
    }

    private void Update()
    {
        if (health <= 0) return; // && agent.remainingDistance > patrolPointRadius) return;

        if (chaseTarget != null)
        {
            ChaseTheTarget();
        }
        else
        {
            GotoNextPoint();
        }

    }

    private void GotoNextPoint()
    {
        if (patrollpoints.Length == 0 || health <= 0 || agent.remainingDistance > patrolPointRadius || doPause)
            return;

        if (doPatrolPause)
        {
            var rndInt = Random.Range(0, 100);
            Debug.Log($"Random integer for patrol pause: {rndInt}");
            if (rndInt < 60)
            {
                doPause = true;
                if (patrolPauseCoroutine != null)
                {
                    StopCoroutine(patrolPauseCoroutine); // Stop any existing coroutine to prevent multiple instances
                }
                patrolPauseCoroutine = StartCoroutine(HandlePause());
            }
        }


        //Debug.Log($"{gameObject.name} is moving to patrol point {destPoint} at position {patrollpoints[destPoint].position}");
        agent.destination = patrollpoints[destPoint].position;
        destPoint = (destPoint + 1) % patrollpoints.Length;
    }

    private IEnumerator HandlePause()
    {
        while (agent.remainingDistance > agent.stoppingDistance)
        {
            yield return null; // Wait until the agent reaches the patrol point
        }

        yield return new WaitForSeconds(patrolPauseDuration); // Pause duration at patrol points
        doPause = false;
    }

    public void ChaseTheTarget()
    {
        var distanceToPlayer = Vector3.Distance(transform.position, chaseTarget.position);
        agent.stoppingDistance = minDistanceToTarget; // Set the stopping distance to the minimum distance to target
        if (distanceToPlayer >= minDistanceToTarget)
        {
            agent.destination = chaseTarget.position;
        }
    }

    protected override void Die()
    {
        agent.isStopped = true; // Stop the NavMeshAgent when the enemy dies
        agent.enabled = false; // Disable the NavMeshAgent component
        col.enabled = false; // Disable the collider to prevent further interactions

        if (DeathAnimations.Length > 1)
        {
            int randomIndex = Random.Range(0, DeathAnimations.Length);
            animator.Play(DeathAnimations[randomIndex].name); // Play a random death animation
        }
        else if (DeathAnimations.Length == 1)
        {
            animator.Play(DeathAnimations[0].name); // Play the single assigned death animation
        }
        else
        {
            Debug.LogWarning($"No death animations assigned to the {gameObject.name}.");
        }

        OnDeath.Invoke(); // Trigger the OnDeath event
    }


    private void CheckPlayerInRange(Collider other)
    {
        if (health <= 0 || chaseTarget != null) return;

        if (other.CompareTag("Player"))
        {
            if (Physics.Raycast(transform.position, other.transform.position - transform.position, out RaycastHit hit))
            {
                if (!hit.collider.CompareTag("Player"))
                {
                    //Debug.Log($"{gameObject.name} cannot see the player due to an obstacle: {hit.collider.name}");
                    return; // Player is not visible, do not chase
                }

                //Debug.Log($"Player detected by {gameObject.name}!");
                chaseTarget = other.transform;
                agent.destination = chaseTarget.position;

                if (patrolPauseCoroutine != null)
                {
                    StopCoroutine(patrolPauseCoroutine); // Stop any existing coroutine to prevent multiple instances
                }
            }

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        CheckPlayerInRange(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (chaseTarget) return;
        CheckPlayerInRange(other);
    }



}