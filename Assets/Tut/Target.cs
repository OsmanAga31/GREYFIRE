using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] protected float health; // Health of the target
    public float Health => health; // Public getter for health

    public void TakeDamage(float damage)
    {
        health -= damage; // Reduce health by the damage amount
        if (health <= 0)
        {
            health = 0;
            Die(); // Call the Die method if health drops to 0 or below
        }
    }

    public void TakeDamage(float damage, DamageType damageType)
    {
        TakeDamage(damage); // Call the basic TakeDamage method to apply the damage
    }

    protected virtual void Die()
    {
        // Add death logic here, such as playing a death animation or spawning effects
        Destroy(gameObject); // Destroy the target object
    }
}