using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Melee : MonoBehaviour
{
    [SerializeField] private Collider collider;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerInput playerInput;

    [Header("Stats")]
    [SerializeField] private int damage = 10;

    private void OnEnable()
    {
        collider.enabled = false;

        playerInput.actions["Melee"].performed += OnMelee;
    }

    private void OnDisable()
    {
        playerInput.actions["Melee"].performed -= OnMelee;
    }

    private void OnMelee(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        collider.enabled = true;
        animator.SetBool("doSwing", false);
        StartCoroutine(DoSwing());
    }

    private IEnumerator DoSwing()
    {
        yield return null; // Wait one frame to ensure the collider is enabled before starting the swing animation
        animator.SetBool("doSwing", true);
        Debug.Log("Swing started, collider enabled.");
    }

    private void OnSwingEnd()
    {
        collider.enabled = false;
        animator.SetBool("doSwing", false);
        Debug.Log("Swing ended, collider disabled.");
    }

    private void OnTriggerEnter(Collider other)
    {
        var target = other.gameObject.GetComponent<Target>();
        if (target != null)
        {
            target.TakeDamage(damage); // Example damage value
            Debug.Log($"Hit {other.gameObject.name} for {damage} damage.");
        }
    }

}
