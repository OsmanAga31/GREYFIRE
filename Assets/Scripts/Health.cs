using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour, IDamagable
{
    [SerializeField] private float health = 100f;
    private float damageMultiplier = 1f; // Multiplier to adjust damage taken based on damage type or other factors
    private Renderer objectRenderer; // Reference to the Renderer component for visual feedback
    
    [SerializeField] private HealthDisplay healthDisplay; // Helper class to manage health display UI
    [SerializeField] private bool isVulnerable = false; // Flag to indicate if the object is currently vulnerable
    [SerializeField] protected Animator animator; // Reference to the Animator component for playing damage animations
    [SerializeField] private float damageEffectDuration = 0.1f; // Duration for visual feedback when taking damage
    
    [Header("Damage Type Settings")]
    [SerializeField] private float BalisitcMultiplier = 1f; // Multiplier for ballistic damage
    [SerializeField] private float ArcaneMultiplier = 1f; // Multiplier for arcane damage
    [SerializeField] private float ExplosiveMultiplier = 1f; // Multiplier for explosive damage
    [SerializeField] private float KineticMultiplier = 1f; // Multiplier for kinetic damage
    [SerializeField] private float MeleeMultiplier = 1f; // Multiplier for melee damage 
    
    public UnityEvent OnDeath; // Event to trigger when the object dies
    
    void Start()
    {
        UpdateHealthDisplay();
        
        OnDeath.AddListener(Die); // Subscribe the Die method to the OnDeath event
        
        animator = GetComponent<Animator>();
        objectRenderer = GetComponentInChildren<Renderer>();
    }
    
    public bool IsVulnerable
    {
        get { return isVulnerable; }
        set { isVulnerable = value; }
    }
    
    public virtual void TakeDamage(float damageAmount, DamageType damageType)
    {
        Debug.Log($"{gameObject.name} took " + damageAmount + " damage!");
        // Implement health reduction logic here
        HandleDamageType(damageType);
        health -= damageAmount * damageMultiplier; // Apply damage multiplier to adjust damage taken
        PlayDamageEffect(); // Play visual feedback for taking damage
        
        if (health <= 0)
        {
            health = 0;
            OnDeath.Invoke(); // Trigger the death event
        }
        
        UpdateHealthDisplay();
    }
    
    public virtual void Heal(float healAmount)
    {
        Debug.Log($"{gameObject.name} healed " + healAmount + " health!");
        // Implement health restoration logic here
        health += healAmount;
        
        UpdateHealthDisplay();
    }
    
    public virtual void Die()
    {
        Debug.Log($"{gameObject.name} has died!");
        PlayDeathAnimation(); // Play death animation
        
        // TODO Implement additional death logic here, such as disabling the object or triggering a respawn
        
    }
    
    public virtual void PlayDamageEffect()
    {
        if (objectRenderer != null)
        {
            // Example: Change the material color to red briefly to indicate damage
            StartCoroutine(FlashDamageEffect());
        }
    }
    
    private System.Collections.IEnumerator FlashDamageEffect()
    {
        Color originalColor = objectRenderer.material.color;
        objectRenderer.material.color = Color.red; // Change to red to indicate damage
        yield return new WaitForSeconds(damageEffectDuration); // Wait for a short duration
        objectRenderer.material.color = originalColor; // Revert to original color
    }
    
    public virtual void PlayDeathAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Die"); // Trigger the damage animation
        }   
    }
    
    public virtual void UpdateHealthDisplay()
    {
        Debug.Log($"{gameObject.name} health updated: " + health);
        if (healthDisplay != null)
        {
            healthDisplay.UpdateHealthDisplay(health);
        }
    }

    private void HandleDamageType(DamageType damageType)
    {
        switch (damageType)
        {
            case DamageType.Ballistic:
                damageMultiplier = BalisitcMultiplier;
                break;
            case DamageType.Arcane:
                damageMultiplier = ArcaneMultiplier;
                break;
            case DamageType.Explosive:
                damageMultiplier = ExplosiveMultiplier;
                break;
            case DamageType.Kinetic:
                damageMultiplier = KineticMultiplier;
                break;
            case DamageType.Melee:
                damageMultiplier = MeleeMultiplier;
                break;
            default:
                damageMultiplier = 1f; // Default multiplier if no specific type is matched
                break;
        }
    }
    
}

