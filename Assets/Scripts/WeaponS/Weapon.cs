using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class Weapon : MonoBehaviour
{
    private static readonly int IsShooting = Animator.StringToHash("isShooting");

    [Header("Visuals")]
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem ps;

    [Header("Weapon Behavior")] 
    //[SerializeField] private bool canHitPlayer = false;
    [SerializeField] private bool lookAtCrosshair = true;
    [SerializeField] private bool autoFire = true;
    [SerializeField] private WeaponData wd40;
    [SerializeField] private Camera mc;
    [SerializeField] private float maxRayDistance;
    [SerializeField] private Transform bulletSpawnPoint;
    private Vector3 GetMaxRayDistance => mc.transform.position + mc.transform.forward * maxRayDistance;
    private Coroutine fireCoroutine;
    
    
    
    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        ps = GetComponent<ParticleSystem>();
        mc =  Camera.main;
    }

    private void FixedUpdate()
    {
        RaycastHit hit;
        if (Physics.Raycast(mc.transform.position, mc.transform.forward, out hit, Mathf.Infinity))
        {
            Debug.DrawRay(mc.transform.position, mc.transform.forward * hit.distance, Color.yellow);
            Debug.Log($"[{((Component)this).gameObject.name}] Hit: {hit.transform.name}");
        }
        else
            Debug.DrawRay(mc.transform.position, mc.transform.forward * 100f, Color.red);
            
        if (lookAtCrosshair)
        {            
            // Smoothly rotate to hit point or max Ray Distance
            Vector3 targetPoint = hit.collider != null ? hit.point : GetMaxRayDistance;
            if (hit.distance < maxRayDistance)
                targetPoint = GetMaxRayDistance;
            Vector3 direction = targetPoint - gameObject.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            gameObject.transform.rotation = Quaternion.Slerp(gameObject.transform.rotation, targetRotation, Time.fixedDeltaTime * wd40.rotationSpeed);
        }


    }

    public void OnFire(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            fireCoroutine = StartCoroutine(Fire());
            animator.SetBool(IsShooting, true);
        }
        else if (ctx.canceled)
        {
            StopCoroutine(fireCoroutine);
            animator.SetBool(IsShooting, false);
        }
    }

    private IEnumerator Fire()
    {
        Shoot();
        while (autoFire)
        {
            yield return new WaitForSeconds(wd40.fireRate);
            Shoot();
        }
    }

    private void Shoot()
    {
        GameObject bul = PoolManager.Instance.GetProjectile(wd40.projectileVariant);
        
        if (!bul) return;
        bul.transform.position = bulletSpawnPoint.position;
        bul.transform.rotation = bulletSpawnPoint.rotation;
        // bul.transform.rotation = bulletSpawnPoint.rotation;
        Projectile bulPro = bul.GetComponent<Projectile>();
        //bulPro.weaponData = wd40;
        bulPro.Shoot();
        
    }
}
