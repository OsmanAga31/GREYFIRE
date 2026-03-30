using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class Gun : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float range;
    [SerializeField] private float fireRate;
    [SerializeField] private float impactForce;

    [SerializeField] private int ammo;
    public int Ammo
    {
        get { return ammo; }
        set { ammo = value; }
    }

    [SerializeField] private Camera fpsCam;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] private GameObject impactEffect;

    private float nextTimeToFire = 0f;


    private bool isShooting;

    private void Start()
    {
        if (ammo <= 0)
        {
            ammo = 10;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (ammo <= 0)
        {
            isShooting = false;
            nextTimeToFire = 0f;

            // play empty click sound or show out of ammo UI here

            Debug.Log($"Weapon {gameObject.name} is out of ammo!");
        }

        if (isShooting && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0, 0, -0.05f), Time.deltaTime * 15);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, Time.deltaTime * 3);
        }
    }

    private void Shoot()
    {
        muzzleFlash.Play();

        ammo--;

        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);
            Target target = hit.transform.GetComponent<Target>();
            Vector3 hitDirection = (hit.point - fpsCam.transform.position).normalized;
            if (target != null)
            {
                target.TakeDamage(damage);
            }

            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForceAtPosition(hitDirection * impactForce, hit.point, ForceMode.Impulse);
            }
            
            //GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(-hitDirection));
            //GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
            GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(Vector3.Reflect(hitDirection, hit.normal)));
        }
    }

    public void StartShooting(CallbackContext ctx)
    {
        if (ctx.performed){
            isShooting = true;
            //Shoot();
        }
        else if (ctx.canceled)
        {
            isShooting = false;
            nextTimeToFire = 0f;
        }
    }

}
