using System;
using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] public WeaponData weaponData; // Reference to the weapon data scriptable object
    [SerializeField] private float speed; // Speed of the projectile
    [SerializeField, Tooltip("Layer mask to specify which layers the projectile can hit")] private LayerMask hitLayers; // Layer mask to specify which layers the projectile can hit
    [SerializeField] private float lifetime; // Time after which the projectile will be destroyed if it doesn't hit anything
    [SerializeField] private Rigidbody rb; // Reference to the Rigidbody component for physics-based movement
    [SerializeField, Tooltip("Whether the projectile should rotate to face its direction of travel, useful for projectiles with a clear front like arrows or rockets")] private bool lookRotation = true; // Whether the projectile should rotate to face its direction of travel, useful for projectiles with a clear front like arrows or rockets
    [SerializeField] private bool isDangerous = true;
    // Hit effects and sounds
    
    private Collider[] results; // Array to store results of area of effect damage checks
    private int lastHintCount = 0;
    
    private void Start()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }

        if (weaponData.spreadSettings.enabled)
            results = new Collider[20];
        
        // Return the projectile after its lifetime expires
        StartCoroutine(ReturnBulletAfterLifeTime());
    }

    private IEnumerator ReturnBulletAfterLifeTime()
    {
        yield return new WaitForSeconds(lifetime);
        ReturnBullet();
    }

    // private void OnEnable()
    // {
    //     Shoot();
    // }

    private void FixedUpdate()
    {
        if (rb.linearVelocity.sqrMagnitude >= 1f && lookRotation)
        {
            // Rotate the projectile to face its direction of travel
            transform.localRotation =  Quaternion.LookRotation(rb.linearVelocity.normalized); 
        }
    }

    public void AddPlayerToHitLayer()
    {
        hitLayers |= (1 << LayerMask.NameToLayer("Player")); // Add the Player layer to the hitLayers mask to allow the projectile to hit player objects
    }

    public void Shoot()
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
    
    private void OnCollisionEnter(Collision other)
    {
        lookRotation = false; // Stop rotating on collision to prevent jittering when hitting objects
        if (!isDangerous) return;
        isDangerous = false; // Set to false to prevent multiple hits from the same projectile
        
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

                foreach (Collider hitCollider in results)
                {
                    DoDamage(other, damage);
                }
            }
            else
            {
                DoDamage(other, damage);
            }

        }
    }

    private void ReturnBullet()
    {
        PoolManager.Instance.ReturnProjectile(weaponData.projectileVariant, gameObject); // Return the projectile to the pool after it hits something
    }

    private void DoDamage(Collision other, float damage)
    {
        // If the hit object has an IDamagable interface, apply damage to it
        if (other.gameObject.TryGetComponent<IDamagable>(out IDamagable damagable))
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
