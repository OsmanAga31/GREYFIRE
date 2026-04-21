using UnityEngine;
public class EnemyMeleeWeapon : MonoBehaviour
{
    public EnemyAttacker enemyAttacker; // Is set by EnemyAttacker in Start() to link the melee weapon to its attacker

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Apply damage to the player
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(enemyAttacker.MeleeAttackDamage, enemyAttacker.MeleeDamageType);
            }
        }
    }

}
