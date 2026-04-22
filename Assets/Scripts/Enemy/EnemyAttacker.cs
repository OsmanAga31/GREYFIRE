using System.Collections;
using UnityEngine;

public class EnemyAttacker : MonoBehaviour
{
    private EnemyMover enemyMover;
    [Header("Ranged Attack Settings")]
    [SerializeField] private bool doRangedAttacks;
    public bool DoRangedAttacks { get => doRangedAttacks; set => doRangedAttacks = value; }
    [SerializeField] private bool RoateWeaponTowardsTarget = true;
    [SerializeField] private DamageType rangedDamageType = DamageType.Ballistic;
    [SerializeField] private float rangedAttackDamage = 10f;
    [SerializeField] private float rangedAttackRange = 10f;
    [SerializeField] private float rangedAttackCooldown = 2f;
    private bool canRangedAttack = true;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform[] projectileSpawnPoints;
    private int currentSpawnPointIndex = 0;
    [SerializeField] private GameObject weaponParent;

    [Header("Melee Attack Settings")]
    [SerializeField] private bool doMeleeAttacks;
    [SerializeField] private DamageType meleeDamageType = DamageType.Melee;
    public DamageType MeleeDamageType => meleeDamageType;
    [SerializeField] private float meleeAttackDamage = 20f;
    public float MeleeAttackDamage => meleeAttackDamage;
    [SerializeField] private float meleeAttackCooldown = 1f;
    private bool canMeleeAttack = true;
    [SerializeField] private EnemyMeleeWeapon meleeWeapon;
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationClip[] meleeAttackAnimations;
    private int animClipIndex = 0;

    private bool saveDoRangedAttacks; // Save the start state of canRangedAttack to restore it later if the enemy is out of range

    private void Start()
    {
        if (doMeleeAttacks && meleeWeapon == null)
        {
            Debug.LogError("Melee attacks enabled but no melee weapon assigned!");
        }
        if (doRangedAttacks && projectilePrefab == null)
        {
            Debug.LogError("Ranged attacks enabled but no projectile prefab assigned!");
        }

        saveDoRangedAttacks = doRangedAttacks; // Save the current state of canRangedAttack to restore it later if the enemy is out of range


        if (meleeWeapon != null)
        {
            meleeWeapon.enemyAttacker = this; // Link the melee weapon to this attacker
        }

        if (!enemyMover)
        {
            enemyMover = transform.GetComponent<EnemyMover>();
            if (!enemyMover)
            {
                Debug.LogError("EnemyAttacker requires an EnemyMover component on the same GameObject.");
            }
        }

        if (!doMeleeAttacks && enemyMover)
            enemyMover.MinDistanceToTarget += 2; // Increase the minimum distance to target for ranged attackers to prevent them from trying to melee attack when they are supposed to be ranged attackers

    }

    private void FixedUpdate()
    {
        HandleRangedAttack();
        HandleMeleeAttack();
    }

    private void HandleRangedAttack()
    {
        if (!doRangedAttacks || !projectilePrefab || projectileSpawnPoints.Length == 0 || !weaponParent)
        {
            if (doRangedAttacks)
                Debug.LogWarning("[SerializeField] Assignment for ranged Attack is missing.");
            return;
        }

        if (enemyMover.Health <= 0f)
        {
            weaponParent.SetActive(false); // Hide the weapon when the enemy is dead
            doRangedAttacks = false;
            this.enabled = false; // Disable the EnemyAttacker component when the enemy is dead to prevent further attacks
            return;
        }

        if (enemyMover.ChaseTarget && enemyMover.DistanceToTarget <= rangedAttackRange)
        {
            if (RoateWeaponTowardsTarget)
            {
                // Rotate Weapon Parent to face the target
                foreach (Transform spawnPoint in projectileSpawnPoints)
                {
                    Vector3 direction = (enemyMover.ChaseTarget.position - spawnPoint.position).normalized;
                    spawnPoint.transform.rotation = Quaternion.LookRotation(direction);
                }
            }
            

            if (canRangedAttack && Physics.Raycast(transform.position, enemyMover.ChaseTarget.position - transform.position, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    HandleShoot();
                }
            }
        }

    }

    private void HandleShoot()
    {
        // Instantiate the projectile and set its direction towards the target
        Projectile projectile = Instantiate(projectilePrefab, projectileSpawnPoints[currentSpawnPointIndex].position, projectileSpawnPoints[currentSpawnPointIndex].rotation).GetComponent<Projectile>();
        projectile.transform.Rotate(90f, 0f, 0f); // Rotate the projectile to face forward
        projectile.isPlayerProjectile = false; // Set the projectile as an enemy projectile
        projectile.Shoot(rangedDamageType, rangedAttackDamage);
        StartCoroutine(HandleRangedCoolDown()); // Start cooldown coroutine

        // Update the spawn point index for the next shot
        currentSpawnPointIndex = (currentSpawnPointIndex + 1) % projectileSpawnPoints.Length;
    }

    private IEnumerator HandleRangedCoolDown()
    {
        canRangedAttack = false; // Prevent attacking during cooldown
        yield return new WaitForSeconds(rangedAttackCooldown);
        canRangedAttack = true; // Allow attacking after cooldown
    }


    private void HandleMeleeAttack()
    {
        if (!doMeleeAttacks || !meleeWeapon || !animator || meleeAttackAnimations.Length == 0)
        {
            if (doMeleeAttacks)
                Debug.LogWarning("[SerializeField] Assignment for melee Attack is missing.");
            return;
        }

        if (enemyMover.Health <= 0f)
        {
            doMeleeAttacks = false;
            meleeWeapon.gameObject.SetActive(false); // Hide the weapon when the enemy is dead
            return;
        }


        if (enemyMover.DistanceToTarget <= enemyMover.MinDistanceToTarget)
        {
            if (!canMeleeAttack)
                return; // Prevent attacking during cooldown
            doRangedAttacks = false; // Disable ranged attacks when performing a melee attack
            meleeWeapon.gameObject.SetActive(true); // Enable the melee weapon when in range
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName(meleeAttackAnimations[animClipIndex].name))
            {
                animator.Play(meleeAttackAnimations[animClipIndex].name);
                animClipIndex = (animClipIndex + 1) % meleeAttackAnimations.Length; // Cycle through attack animations
                StartCoroutine(DisableMeleeWeaponAfterAnimation()); // Start coroutine to disable the melee weapon after the animation
                StartCoroutine(HandleMeleeCoolDown()); // Start cooldown coroutine
            }
        }
        else
        {
            doRangedAttacks = saveDoRangedAttacks; // Restore the previous state of canRangedAttack when out of melee range
        }

    }

    private IEnumerator DisableMeleeWeaponAfterAnimation()
    {
        yield return new WaitForSeconds(meleeAttackAnimations[animClipIndex].length);
        meleeWeapon.gameObject.SetActive(false); // Disable the melee weapon after the animation finishes
    }

    private IEnumerator HandleMeleeCoolDown()
    {
        canMeleeAttack = false; // Prevent attacking during cooldown
        yield return new WaitForSeconds(meleeAttackCooldown);
        canMeleeAttack = true; // Allow attacking after cooldown
    }



}
