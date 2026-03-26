using UnityEngine;

public class Target : MonoBehaviour
{
    [SerializeField] private float health; // Health of the target

    public void TakeDamage(float damage)
    {
        health -= damage; // Reduce health by the damage amount
        if (health <= 0)
        {
            Die(); // Call the Die method if health drops to 0 or below
        }
    }

    private void Die()
    {
        // Add death logic here, such as playing a death animation or spawning effects
        Destroy(gameObject); // Destroy the target object
    }
}