using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private DamageType damageType = DamageType.Ballistic; // Type of damage dealt by the projectile, used for damage calculations and resistances
    public GameObject hitEffect; // Particle system to play on hit
    [SerializeField] private float damage; // Damage dealt by the projectile on hit
    [SerializeField] private float speed; // Speed of the projectile

    [SerializeField, Tooltip("Layer mask to specify which layers the projectile can hit")] 
    private LayerMask hitLayers; // Layer mask to specify which layers the projectile can hit
    
    [SerializeField] private float lifetime; // Time after which the projectile will be destroyed if it doesn't hit anything
    [SerializeField] private Rigidbody rb; // Reference to the Rigidbody component for physics-based movement
   
    [SerializeField, Tooltip("Whether the projectile should rotate to face its direction of travel, useful for projectiles with a clear front like arrows or rockets")] 
    private bool lookRotation = true; // Whether the projectile should rotate to face its direction of travel, useful for projectiles with a clear front like arrows or rockets
    
    [SerializeField] private bool isDangerous = true;
    [SerializeField] private bool destroyOnHit = true; // Whether the projectile should be destroyed on hit, set to false for projectiles that should persist after hitting something (like piercing bullets or grenades that explode after a delay)
    public bool isPlayerProjectile = false; // Whether the projectile was fired by the player

    private void Start()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        // Return the projectile after its lifetime expires
        StartCoroutine(DestroyBulletAfterLifeTime());
    }

    private IEnumerator DestroyBulletAfterLifeTime()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject, lifetime);
    }

    // private void OnEnable()
    // {
    //     Shoot();
    // }

    private void FixedUpdate()
    {
        if (lookRotation) //&& rb.linearVelocity.sqrMagnitude >= 1f)
        {
            // Rotate the projectile to face its direction of travel
            Vector3 direction = rb.linearVelocity.normalized;
            transform.localRotation =  direction == Vector3.zero ? Quaternion.identity : Quaternion.LookRotation(direction);
        }
    }

    public void AddPlayerToHitLayer()
    {
        hitLayers |= (1 << LayerMask.NameToLayer("Player")); // Add the Player layer to the hitLayers mask to allow the projectile to hit player objects
    }

    public void Shoot()
    {
        // Apply force to the projectile forward based on its speed with Rigidbody physics
        rb.linearVelocity = transform.up * speed;
        //rb.linearVelocity = direction.normalized * speed;

        //Vector3 direction = rb.linearVelocity.normalized;
        //transform.localRotation = direction == Vector3.zero ? Quaternion.identity : Quaternion.LookRotation(direction);
    }

    public void Shoot(DamageType damageType, float damage)
    {
        this.damageType = damageType;
        this.damage = damage;
        Shoot();
    }

    private void OnTriggerEnter(Collider other)
    {
        lookRotation = false; // Stop rotating on collision to prevent jittering when hitting objects
        if (!isDangerous) return;

        // Check if the projectile hits an object on the specified layers
        if ((hitLayers.value & (1 << other.gameObject.layer)) > 0)
        {
            DoDamage(other, damage);
        }

        // Play hit effect if assigned
        if (hitEffect != null)
        {
            Instantiate(hitEffect, other.ClosestPoint(transform.position), Quaternion.identity);
        }

        // Destroy the projectile after hitting something
        if (!destroyOnHit) return;

        Destroy(gameObject);
        isDangerous = false; // Set to false to prevent multiple hits from the same projectile
    }

    private void DoDamage(Collider other, float damage)
    {
        if (isPlayerProjectile)
        {
            if (other.gameObject.TryGetComponent<Target>(out Target target))
            {
                target.TakeDamage(damage, damageType);
            }
        }
        else
        {
            if (other.gameObject.TryGetComponent<Health>(out Health health))
            {
                health.TakeDamage(damage, damageType);
            }
        }
    }
    private void DoDamage(Collider other, float damage, DamageType damageType)
    {
        // If the hit object has an IDamagable interface, apply damage to it
        if (other.gameObject.TryGetComponent<Health>(out Health health))
        {
            health.TakeDamage(damage, damageType);
        }
    }

}
