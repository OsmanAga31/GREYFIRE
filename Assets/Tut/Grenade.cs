using UnityEngine;

public class Grenade : MonoBehaviour
{
    [SerializeField] private float delay = 3f;
    [SerializeField] private float radius = 5f;
    [SerializeField] private float force = 700f;
    [SerializeField] private float damage = 50f;
    private bool isTimerActive = false;
    public bool IsTimerActive
    {
        get { return isTimerActive; }
        set { isTimerActive = value; }
    }

    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip throwClip;
    [SerializeField] private AudioClip impactClip;
    [SerializeField] private AudioClip[] explosionClip;

    private float countdown;
    bool hasExploded = false;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        countdown = delay;
        audioSource.clip = throwClip;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isTimerActive) return;

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

        // Play explosion sound
        audioSource.clip = explosionClip[Random.Range(0, explosionClip.Length-1)];
        audioSource.Play();

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

        Despawn();
    }

    private void Despawn()
    {
        transform.GetComponent<MeshRenderer>().enabled = false;
        transform.GetComponent<SphereCollider>().enabled = false;

        // Remove grenade
        Destroy(gameObject, audioSource.clip.length);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (audioSource.clip != impactClip)
            audioSource.clip = impactClip;

        // Play sound on collision
        audioSource.Play();
    }



}