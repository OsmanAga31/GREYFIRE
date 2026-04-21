using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class Gun : MonoBehaviour, IItemAdder
{
    [SerializeField] protected float damage;
    [SerializeField] protected float range;
    [SerializeField] protected float fireRate;
    [SerializeField] protected float impactForce;
    [SerializeField] protected AmmoType ammoType;

    [SerializeField] private int ammo;
    [SerializeField] protected PlayerInput playerInput;

    public int Ammo
    {
        get { return ammo; }
        set
        {
            ammo = value;
        }
    }

    [Header("VFX")]
    [Header("Visual")]
    [SerializeField] protected Camera fpsCam;
    [SerializeField] private ParticleSystem muzzleFlash;
    [SerializeField] protected GameObject impactEffect;
    [SerializeField] protected TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private Vector3 startPosition;

    [Header("Visual")]
    [SerializeField] private AudioSource shootAudioSource;
    [SerializeField] private AudioClip[] shootClip;
    [SerializeField] private AudioClip emptyClip;

    private float nextTimeToFire = 0f;


    protected bool isShooting;

    protected virtual void Start()
    {
        startPosition = transform.localPosition;

        if (ammo <= 0)
        {
            ammo = 10;
        }
        ammoText.text = ammo.ToString();
    }

    protected virtual void OnEnable()
    {
        isShooting = false;
        nextTimeToFire = 0f;
        ammoText.text = ammo.ToString();
        weaponNameText.text = gameObject.name;

        playerInput.actions["Attack"].performed += StartShooting;
        playerInput.actions["Attack"].canceled += StartShooting;
    }

    protected virtual void OnDisable()
    {
        playerInput.actions["Attack"].performed -= StartShooting;
        playerInput.actions["Attack"].canceled -= StartShooting;

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        ShootCheck();

        if (isShooting && Time.time >= nextTimeToFire)
        {
            if ( shootAudioSource && shootClip.Length > 0)
                shootAudioSource.PlayOneShot(shootClip[Random.Range(0, shootClip.Length)]);

            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
            transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(0, 0, -0.05f) + startPosition, Time.deltaTime * 15);
           
        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, startPosition, Time.deltaTime * 3);
        }
    }

    protected virtual void ShootCheck()
    {
        if (isShooting && CustomCheck())
        {
            isShooting = false;
            nextTimeToFire = 0f;

            // play empty click sound or show out of ammo UI here
            Debug.Log($"Weapon {gameObject.name} is out of ammo!");
            if (shootAudioSource && emptyClip)
                shootAudioSource.PlayOneShot(emptyClip);
        }
    }

    protected virtual bool CustomCheck()
    {
        // 
        return ammo <= 0;
    }

    protected virtual void Shoot()
    {
        if ( muzzleFlash )
            muzzleFlash.Play();

        if ( ammo > 0)
        {
            ammo--;
            ammoText.text = ammo.ToString();
        }

        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range, ~LayerMask.GetMask("Player")))
        {
            Debug.Log("hitted: " + hit.transform.name);
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
            
            GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(Vector3.Reflect(hitDirection, hit.normal)));
        }

    }

    public virtual void StartShooting(CallbackContext ctx)
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

    public void Add(int amount, AmmoType type)
    {
        Ammo += amount;
        if ((int)type == (int)ammoType)
        {
            ammoText.text = Ammo.ToString();
        }
    }
}
