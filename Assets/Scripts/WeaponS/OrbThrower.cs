using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder.Shapes;

public class OrbThrower : GrenadeThrower
{
    [SerializeField] private float manaCost = 50f; // Mana cost for throwing an orb
    [SerializeField] private bool canThrowWithoutMana = false; // Option to allow throwing without mana

    protected override void OnEnable()
    {
        playerInput.actions["AltAttack"].performed += OnThrow;
    }

    protected override void OnDisable()
    {
        playerInput.actions["AltAttack"].performed -= OnThrow;
    }

    protected override IEnumerator ThrowGrenade(bool performThrow)
    {
        var canThrow = canThrowWithoutMana || Mana.Instance.ConsumeMana(manaCost);
        return base.ThrowGrenade(canThrow); 
    }

    protected override void ThrowGrenadeHelper(GameObject grenade, Rigidbody rb, Grenade gr)
    {
        grenade.transform.parent = null; // Detach the grenade from the hand
        rb.isKinematic = false;
        RaycastHit hit;
        var throwDirection = Vector3.zero;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 100f))
        {
            throwDirection = hit.point;
        }
        else
        {
            throwDirection = transform.position + transform.forward * 100f; // Default to forward if no hit
        }
        grenade.transform.LookAt(throwDirection); // Make the grenade face the throw direction
        grenade.transform.Rotate(90f, 0f, 0f); // Rotate the grenade to align with the throw direction
        var projectile = grenade.GetComponent<Projectile>();
        projectile.isPlayerProjectile = true; // Set the projectile as a player projectile
        projectile.Shoot(); // Call the Shoot method to apply the force

    }
}
