using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class KeyCardMover : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent; // Reference to the NavMeshAgent component
    public Transform targetPos;
    [SerializeField] private CardSwipe cardSwipe; // Reference to the CardSwipe script

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (agent == null)
        {
            Debug.LogError("NavMeshAgent component is not assigned.");
            return;
        }
        if (cardSwipe == null)
        {
            Debug.LogError("CardSwipe script reference is not assigned.");
            return;
        }

        cardSwipe.OnCardReachedKeyPad.AddListener(DisableSelf); // Subscribe to the event

        StartCoroutine(SetAgentTargetDelayed()); // Start the coroutine to set the agent's destination after a delay
    }

    private IEnumerator SetAgentTargetDelayed()
    {
        yield return null; // Wait for 1 frame before setting the destination
        if (targetPos == null)
        {
            Debug.LogError("Target position is not assigned.");
            yield break; // Exit the coroutine if targetPos is not assigned
        }
        agent.SetDestination(targetPos.position); // Set the destination to the target position
    }

    private void OnDisable()
    {
        cardSwipe.OnCardReachedKeyPad.RemoveListener(DisableSelf); // Unsubscribe from the event when the object is disabled to prevent memory leaks
    }

    private void DisableSelf()
    {
        cardSwipe.OnCardReachedKeyPad.RemoveListener(DisableSelf); // Unsubscribe from the event to prevent memory leaks
        transform.DetachChildren();
        gameObject.SetActive(false); // Disable the GameObject when the event is triggered
    }
}
