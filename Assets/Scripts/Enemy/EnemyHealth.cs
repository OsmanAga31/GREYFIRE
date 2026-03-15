using UnityEngine;

public class EnemyHealth : Health
{
    private static readonly int PushedTrigger = Animator.StringToHash("pushedTrigger");
    private static readonly int DeadTrigger = Animator.StringToHash("deadTrigger");
    [SerializeField] private DamageNumbers damageNumbers; // Reference to the DamageNumbers component for displaying damage feedback

    void Start()
    {
        damageNumbers = GetComponent<DamageNumbers>();
    }

    public override void PlayDamageEffect()
    {
        animator.SetTrigger(PushedTrigger); // Trigger the "pushedTrigger" animation when the enemy takes damage
    }
    
    public override void TakeDamage(float damageAmount, DamageType damageType)
    {
        base.TakeDamage(damageAmount, damageType);
        animator.SetTrigger(PushedTrigger); // Trigger the "pushedTrigger" animation when the enemy takes damage
        Vector3 position = transform.position + new Vector3(Random.Range(-1.5f, 1.5f), Random.Range(2f, 3f), Random.Range(-1.5f, 1.5f)); // Adjust the position to display damage numbers above the enemy and add a random offset
        damageNumbers.PlayDamageEffect(position, damageAmount); // Display damage numbers at the enemy's position
    }

    public override void PlayDeathAnimation()
    {
        animator.SetTrigger(DeadTrigger); // Trigger the "dieTrigger" animation when the enemy dies
    }
}
