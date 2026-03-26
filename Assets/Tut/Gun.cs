using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class Gun : MonoBehaviour
{
    [SerializeField] private float damage;
    [SerializeField] private float range;
    [SerializeField] private float fireRate;

    [SerializeField] private Camera fpsCam;
    [SerializeField] private ParticleSystem muzzleFlash;


    private bool isShooting;
    // Update is called once per frame
    void FixedUpdate()
    {
        //if (isShooting)
        //    Shoot();
    }

    private void Shoot()
    {
        muzzleFlash.Play();

        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            Debug.Log(hit.transform.name);
            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }
        }
    }

    public void StartShooting(CallbackContext ctx)
    {
        if (ctx.performed){
            isShooting = true;
            Shoot();
        }
        else if (ctx.canceled)
            isShooting = false;
    }

}
