using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class DroneWakeUp : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent navMeshAgent;
    private float saveNavMeshSpeed = 0;
    [SerializeField] private EnemyMover enemyMover;
    [SerializeField] private EnemyAttacker enemyAttacker;

    private void Start()
    {
        // Disable the EnemyMover and EnemyAttacker components at the start
        saveNavMeshSpeed = navMeshAgent.speed;
        navMeshAgent.speed = 0;
        enemyAttacker.enabled = false;
        enemyMover.enabled = false;
        // Start the wake-up sequence
        StartCoroutine(WakeUpSequence());
    }

    private IEnumerator WakeUpSequence()
    {
        // Wait for the animation to finish (assuming it has a known duration)
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        // Enable the EnemyMover and EnemyAttacker components
        enemyAttacker.enabled = true;
        navMeshAgent.speed = saveNavMeshSpeed; // Restore the original speed of the NavMeshAgent
        enemyMover.enabled = true;
    }

}
