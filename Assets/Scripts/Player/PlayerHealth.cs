using UnityEngine;

public class PlayerHealth : Health
{
    
    public void TakeDamage(float damageAmount, DamageType damageType)
    {
        base.TakeDamage(damageAmount, damageType);
        Debug.Log("The player was wounded and took " + damageAmount + " damage!");
        // TODO Implement health reduction logic here, such as updating a health bar or checking for player death
    }
    
    public void Heal(float healAmount)
    {
        base.Heal(healAmount);
        Debug.Log("The player was healed and restored " + healAmount + " health!");
        // TODO Implement health restoration logic here, such as updating a health bar
    }
    
    public void Die()
    {
        base.Die();
        Debug.Log("The player has died!");
        // TODO Implement player death logic here, such as triggering a death animation or restarting the level
    }
        
        
    
}
