using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class GunMagic : Gun
{
    [SerializeField] private Animator animator;
    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform projectileSpawnPoint;

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
        Ammo--;
        ammoText.text = Ammo.ToString();

        if (!animator) return;
        animator.SetTrigger("isShooting");

        RaycastHit hit;
        Vector3 hitPos;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
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

