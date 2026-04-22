using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using static UnityEngine.InputSystem.InputAction;

public class GunMagic : Gun
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform projectileSpawnPoint;

    [Header("Projectile Settings")]
    [SerializeField] private float projDelay;

    [Header("Mana Settings")]
    [SerializeField] private float manaCost = 10f; // Mana cost for throwing an orb
    [SerializeField] private bool canShootWithoutMana = false; // Option to allow shooting without mana

    protected override void Start()
    {
        base.Start();

        animator = GetComponent<Animator>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (!animator) return;
        animator.SetBool("isWalking", false);

        playerInput.actions["Move"].performed += OnMove;
        playerInput.actions["Move"].canceled += OnMove;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        playerInput.actions["Move"].performed -= OnMove;
        playerInput.actions["Move"].canceled -= OnMove;
    }

    protected override void Shoot()
    {
        if (Ammo > 0)
        {
            Ammo--;
            ammoText.text = Ammo.ToString();
        }
        else if (!canShootWithoutMana && Mana.Instance.CurrentMana >= manaCost)
        {
            Mana.Instance.ConsumeMana(manaCost);
        }

        if (!animator) return;
        animator.SetTrigger("isShooting");

        StartCoroutine(ProjectileShootDelayed());
    }

    protected override bool CustomCheck()
    {

        if (Ammo > 0 || canShootWithoutMana || Mana.Instance.CurrentMana >= manaCost )
            return false; 
        return true; 
    }

    private IEnumerator ProjectileShootDelayed()
    {
        yield return new WaitForSeconds(projDelay); // Delay to sync with shooting animation
        RaycastHit hit;
        Vector3 hitPos;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range, ~LayerMask.GetMask("Player")))
        {
            //Debug.Log("hitted: " + hit.transform.name);
            if (hit.collider.CompareTag("Bullet"))
            {
                hitPos = transform.position + transform.forward * range; // If it hits another bullet, shoot straight ahead
            }
            hitPos = hit.point;
        }
        else
        {
            hitPos = fpsCam.transform.position + fpsCam.transform.forward * range;
        }

        GameObject projectileGO = Instantiate(projectile, projectileSpawnPoint.position, Quaternion.identity);
        projectileGO.transform.LookAt(hitPos);
        projectileGO.transform.Rotate(90f, 0f, 0f); // Rotate the projectile to face the correct direction


        Projectile proj = projectileGO.GetComponent<Projectile>();
        proj.Shoot();
        proj.hitEffect = impactEffect;
    }

    private void OnMove(CallbackContext ctx)
    {
        if (ctx.performed)
            animator.SetBool("isWalking", true);
        else if (ctx.canceled)
            animator.SetBool("isWalking", false);
        
    }

}

