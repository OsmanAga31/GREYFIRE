using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] bulletPrefabs; // Array of bullet prefabs to choose from
    [SerializeField] int BPindex = 0; // Index to track which bullet prefab to spawn
    
    [SerializeField] WeaponData[] weaponDatas; // Array of weapon data scriptable objects to choose from
    [SerializeField] int WDindex = 0; // Index to track which weapon data to use for the spawned bullets
    
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
        
        GameObject bulletPrefab = bulletPrefabs[BPindex];
        bulletPrefab.GetComponent<Projectile>().weaponData = weaponDatas[WDindex]; // Assign the selected weapon data to the bullet's Projectile component
        
        // handle weaponData spreadcount
        if (weaponDatas[WDindex].spreadSettings.enabled)
        {
            // If spread is enabled, spawn multiple bullets based on the spread count
            for (int i = 0; i < weaponDatas[WDindex].spreadSettings.count; i++)
            {
                Instantiate(bulletPrefab, transform.position, transform.rotation);
            }
        } 
        else
            // Instantiate the selected bullet prefab at the spawner's position and rotation
            Instantiate(bulletPrefab, transform.position, transform.rotation);
    }
    
    
}
