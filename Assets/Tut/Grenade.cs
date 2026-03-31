using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private float delay = 3f;
    [SerializeField] private float radius = 5f;
    [SerializeField] private float force = 700f;
    [SerializeField] private float damage = 50f;

    [SerializeField] private GameObject explosionEffect;

    private float countdown;
    bool hasExploded = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        countdown = delay;
    }

    // Update is called once per frame
    void Update()
    {
        countdown -= Time.deltaTime;
        if (countdown <= 0f && !hasExploded)
        {
            Explode();
            hasExploded = true;
        }
    }

    private void Explode()
    {
        // Showw effect
        Instantiate(explosionEffect, transform.position, transform.rotation);

        // Get nearby objects
        Collider[] collidersToDestroy = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider nearbyObject in collidersToDestroy)
        {
            // Damage
            Destructable dest = nearbyObject.GetComponent<Destructable>();
            if (dest != null)
            {
                dest.Destroy();
            }
            Target target = nearbyObject.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }
        }

        Collider[] collidersToMove = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider nearbyObject in collidersToMove)
        {
            // Add force
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(force, transform.position, radius);
            }
        }

        // Remove grenade
        Destroy(gameObject);
    }
}