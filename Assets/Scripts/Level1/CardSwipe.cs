using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class CardSwipe : MonoBehaviour
{
    public UnityEvent OnCardReachedKeyPad;
    [SerializeField] private BoxCollider cardCollider; // Reference to the BoxCollider component
    private Vector3 colSaveSize; // Variable to store the original size of the BoxCollider
    [SerializeField] private Rotator[] rotators; // Array of Rotator components to rotate the card

    [Header("Swipe Settings")]
    [SerializeField] private Transform point1;
    [SerializeField] private Transform point2;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float waitTimeBeforeMoving = 1f;
    [SerializeField] private float waitTimeAtPoint1 = 2f;
    [SerializeField] private float waitTimeToOpenGate = 2f;
    private bool canOpenGate = false;
    [SerializeField] private GateOpener gateOpener;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (cardCollider == null)
        {
            Debug.LogError("BoxCollider component is not assigned.");
            return;
        }
        colSaveSize = cardCollider.size; // Store the original size of the BoxCollider
        cardCollider.size = Vector3.one; // Set the size of the BoxCollider to (1, 1, 1)

    }

    private IEnumerator MoveSmoothlyToPoint1()
    {
        yield return new WaitForSeconds(waitTimeBeforeMoving); // Wait before starting to move
        while (Vector3.Distance(transform.position, point1.position) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, point1.position, Time.deltaTime * moveSpeed);
            transform.rotation = Quaternion.Lerp(transform.rotation, point1.rotation, Time.deltaTime * rotationSpeed);
            yield return null; // Wait for the next frame
        }
        StartCoroutine(MoveSmoothlyToPoint2()); // Start moving to point 2 after reaching point 1
    }

    private IEnumerator MoveSmoothlyToPoint2()
    {
        yield return new WaitForSeconds(waitTimeAtPoint1); // Wait at point 1 before moving to point 2
        while (Vector3.Distance(transform.position, point2.position) > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, point2.position, Time.deltaTime * moveSpeed);
            yield return null; // Wait for the next frame
        }
        StartCoroutine(WaitAndOpenGate()); // Start waiting to open the gate after reaching point 2
    }

    private IEnumerator WaitAndOpenGate()
    {
        yield return new WaitForSeconds(waitTimeToOpenGate); // Wait before opening the gate
        gateOpener.OpenGate(); // Open the gate after waiting
        cardCollider.isTrigger = false; // Disable the trigger to allow physical interactions
        Rigidbody rb = GetComponent<Rigidbody>();
        yield return new WaitForSeconds(1f); // Wait a moment before enabling physics to ensure the gate has opened
        if (rb != null)
        {
            rb.isKinematic = false; // Make it fall
            rb.interpolation = RigidbodyInterpolation.Interpolate; // Smooth the movement of the rigidbody
            rb.AddForce(new Vector3(0f, 2f, 1f) * 3f, ForceMode.Impulse); // Throw the card 
            rb.AddTorque(Random.insideUnitSphere * 2f, ForceMode.Impulse); // Add some random rotation for visual effect
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("KeyPad"))
            return;

        if (!canOpenGate)
        {
            foreach (var rotator in rotators)
            {
                rotator.enabled = false; // Disable the Rotator components to stop rotating the card
            }
            OnCardReachedKeyPad.Invoke(); // Trigger the event when the card reaches the keypad
            cardCollider.size = colSaveSize;
            var gateOpenerTmp = other.transform.GetComponent<GateOpener>();
            point1 = gateOpenerTmp.point1; // Set point 1 from the GateOpener
            point2 = gateOpenerTmp.point2; // Set point 2 from the GateOpener
            gateOpener = gateOpenerTmp; // Set the reference to the GateOpener
            StartCoroutine(MoveSmoothlyToPoint1()); // Start moving to point 1 when the card reaches the keypad
            canOpenGate = true; // Set the flag to allow opening the gate after the card has reached the keypad
        }
        else
        {
            gateOpener.OpenGate(); // Open the gate if the card has already reached the keypad

        }
    }
}
