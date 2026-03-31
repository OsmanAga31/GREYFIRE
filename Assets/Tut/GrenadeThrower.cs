using TMPro;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class GrenadeThrower : MonoBehaviour, IItemAdder
{
    [SerializeField] private float throwForce = 30;
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private Vector3 torque;

    [SerializeField] private TextMeshProUGUI grenadeText;
    [SerializeField] private int grenadeAmount = -1;
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
    }

    public void OnThrow(CallbackContext ctx)
    {
        if (!ctx.performed || grenadeAmount <= 0) return;
        ThrowGrenade();
        grenadeAmount--;
    }

    private void ThrowGrenade()
    {
        GameObject grenade = Instantiate(grenadePrefab, transform.position, grenadePrefab.transform.rotation);
        Rigidbody rb = grenade.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(transform.forward * throwForce, ForceMode.VelocityChange);
            rb.AddTorque(torque, ForceMode.VelocityChange);
        }
    }

    public void Add(int amount)
    {
        GrenadeAmount += amount;
    }

}
