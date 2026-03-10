using UnityEngine;

public class TestBullet : MonoBehaviour
{
    public void OnCollisionEnter(Collision collision)
    {
        // Check if the collided object has a PlayerHealth component
        PlayerHealth playerHealth = collision.gameObject.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            // If it does, apply damage to the player
            playerHealth.TakeDamage(10f, DamageType.Ballistic); // Example damage amount and type
            Debug.Log("Bullet hit the player and dealt 10 damage!");
        }
        
        // Destroy the bullet after it collides with something
        Destroy(gameObject);
    }
}
