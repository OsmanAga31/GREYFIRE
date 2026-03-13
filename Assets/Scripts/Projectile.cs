using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] public WeaponData weaponData; // Reference to the weapon data scriptable object
    [SerializeField] private float speed = 20f; // Speed of the projectile
    [SerializeField] private LayerMask hitLayers; // Layer mask to specify which layers the projectile can hit
    [SerializeField] private float lifetime = 0.5f; // Time after which the projectile will be destroyed if it doesn't hit anything
    [SerializeField] private Rigidbody rb; // Reference to the Rigidbody component for physics-based movement
    // Hit effects and sounds
    
    private Collider[] results = new Collider[20]; // Array to store results of area of effect damage checks
    private int lastHintCount = 0;
    
    private void Start()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        rb.useGravity = false; // Disable gravity for the projectile to ensure it travels in a straight line
        
        // Destroy the projectile after its lifetime expires
        Destroy(gameObject, lifetime);
    }

    private void OnEnable()
    {
        Shoot();
    }

    private void Shoot()
    {
        if (weaponData.spreadSettings.enabled)
        {
            // Apply random spread to the projectile's direction based on the weapon's spread settings
            float spreadX = UnityEngine.Random.Range(-weaponData.spreadSettings.angle, weaponData.spreadSettings.angle);
            float spreadY = UnityEngine.Random.Range(-weaponData.spreadSettings.angle, weaponData.spreadSettings.angle);
            Vector3 spreadDirection = transform.forward + new Vector3(spreadX, spreadY, 0f);
            rb.linearVelocity = spreadDirection.normalized * speed;
        }
        else
            // Apply force to the projectile forward based on its speed with Rigidbody physics
            rb.linearVelocity = transform.forward * speed;
            
    }
    
    private void OnTriggerEnter(Collider other)
    {
        float damage = weaponData.baseDamage; // Get the base damage from the weapon data
        
        if (weaponData.spreadSettings.enabled)
        {
            damage /= weaponData.spreadSettings.count; // If spread is enabled, divide the damage by the number of projectiles to balance total damage output
        }
        
        // Check if the projectile hits an object on the specified layers
        if ((hitLayers.value & (1 << other.gameObject.layer)) > 0)
        {

            if (weaponData.aoeSettings.enabled)
            {
                // Perform an area of effect damage around the hit point
                lastHintCount = Physics.OverlapSphereNonAlloc(transform.position, weaponData.aoeSettings.radius, results, hitLayers);

                // visualize the sphere
                
                
                
                foreach (Collider hitCollider in results)
                {
                    DoDamage(other, damage);
                }
            }
            else
            {
                DoDamage(other, damage);
            }
            
            // Destroy the projectile upon hitting an object
            Destroy(gameObject);
        }
    }

    private void DoDamage(Collider other, float damage)
    {
        // If the hit object has an IDamagable interface, apply damage to it
        if (other.TryGetComponent<IDamagable>(out IDamagable damagable))
        {
            damagable.TakeDamage(damage, weaponData.damageType);
        }
    }

    private void OnDrawGizmos()
    {
        if (lastHintCount < 1) return;
        
        Gizmos.color = lastHintCount > 0 ? new Color(1f, 0f, 0f, 0.4f) : new Color(0f, 1f, 0f, 0.2f); // Red for AOE hits, yellow for single hits
        
        Gizmos.DrawSphere(transform.position, weaponData.aoeSettings.radius); // Draw a sphere to visualize the area of effect damage radius
    }
}
