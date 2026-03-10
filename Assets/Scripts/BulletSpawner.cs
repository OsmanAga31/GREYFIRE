using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] bulletPrefabs; // Array of bullet prefabs to choose from
    [SerializeField] int index = 0; // Index to track which bullet prefab to spawn
    
    [SerializeField] bool paused = false; // Flag to control whether the spawner is paused
    [SerializeField] float spawnInterval = 1f; // Time interval between spawns
    
    void Start()
    {
        // Start the spawning process
        InvokeRepeating(nameof(SpawnBullet), spawnInterval, spawnInterval);
    }
    
    void SpawnBullet()
    {
        if (paused) return; // If paused, do not spawn bullets
        
        GameObject bulletPrefab = bulletPrefabs[index];
        
        // Instantiate the selected bullet prefab at the spawner's position and rotation
        Instantiate(bulletPrefab, transform.position, transform.rotation);
    }
    
    
}
