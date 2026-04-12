using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class GrenadeThrower : MonoBehaviour, IItemAdder
{
    [SerializeField] private GrenadeThrower otherGT; // Reference to the other GrenadeThrower so they do not interfere with each other when throwing 
    [SerializeField] private float throwForce = 30;
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private Vector3 torque;

    [SerializeField] private GameObject throwHand; 
    [SerializeField] private float throwDelay = 0.5f; // Delay to sync with throw animation
    [SerializeField] private float throwOffset = 0.5f; // Upward offset to create a more natural arc
    private Coroutine throwCoroutine;
    private bool isThrowing;
    public bool IsThrowing => isThrowing;


    [SerializeField] private TextMeshProUGUI grenadeText;
    [SerializeField] private int grenadeAmount = -1;
    [SerializeField] protected PlayerInput playerInput;
    [SerializeField] private Animator HandLAnimator;
    public int GrenadeAmount
    {
        get { return grenadeAmount; }
        set
        {
            grenadeAmount = value;
            if (grenadeText != null)
            {
                grenadeText.text = grenadeAmount.ToString();
            }
        }
    }

    private void Start()
    {
        if (grenadeAmount <= 0)
        {
            grenadeAmount = 3;
        }

        if (grenadeText != null)
        {
            grenadeText.text = grenadeAmount.ToString();
        }
    }

    protected virtual void OnEnable()
    {
        playerInput.actions["Grenade"].performed += OnThrow;
    }

    protected virtual void OnDisable()
    {
        playerInput.actions["Grenade"].performed -= OnThrow;
    }

    public void OnThrow(CallbackContext ctx)
    {
        if (!ctx.performed 
            || grenadeAmount <= 0 
            || throwCoroutine != null
            || HandLAnimator.GetCurrentAnimatorStateInfo(0).IsName("Throw")
            || (otherGT != null && otherGT.IsThrowing)
            ) return;

        isThrowing = true;
        throwCoroutine = StartCoroutine(ThrowGrenade(true));
    }

    protected virtual IEnumerator ThrowGrenade(bool performThrow)
    {
        if (!performThrow) yield break;

        grenadeAmount--;
        if (grenadeText != null)
            grenadeText.text = grenadeAmount.ToString();

        GameObject grenade = Instantiate(grenadePrefab, throwHand.transform.position, grenadePrefab.transform.rotation);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        Grenade gr = grenade.GetComponent<Grenade>();

        grenade.transform.SetParent(throwHand.transform); // Parent the grenade to the hand for animation
        rb.isKinematic = true; // Make the grenade kinematic while it's in the hand
        HandLAnimator.SetTrigger("Throw");

        yield return new WaitForSeconds(throwDelay); // Delay to sync with throw animation
        if (rb != null)
        {
            ThrowGrenadeHelper(grenade, rb, gr);
        }
        yield return new WaitForSeconds(throwDelay);
        throwCoroutine = null;
        isThrowing = false;
    }

    protected virtual void ThrowGrenadeHelper(GameObject grenade, Rigidbody rb, Grenade gr)
    {
        grenade.transform.parent = null; // Detach the grenade from the hand
        var throwDirection = transform.forward + Vector3.up * throwOffset; // Add some upward force for a more natural arc
        rb.isKinematic = false; // Make the grenade affected by physics again
        rb.AddForce(throwDirection * throwForce, ForceMode.VelocityChange);
        rb.AddTorque(torque, ForceMode.VelocityChange);
        gr.IsTimerActive = true; // Start the grenade's timer after throwing
    }

    public void Add(int amount, AmmoType type)
    {
        GrenadeAmount += amount;
    }

}
