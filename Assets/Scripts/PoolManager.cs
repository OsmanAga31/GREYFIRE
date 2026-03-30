using System;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileVariant
{
    BulletFoam,
    BulletFoamThick,
    SpellShot,
    Grenade,
    KineticCharge,
    ArcaneOrb,
    

    HitEffectDust
}

[Serializable]
public struct ProjectileEntry
{
    public ProjectileVariant variant; // The type of projectile
    public GameObject prefab; // The prefab associated with this projectile type
    public int poolSize; // Array to specify the size of the pool for each projectile variant
}

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;
    
    [SerializeField] private ProjectileEntry[] projectileEntries; // Array to hold the projectile variants and their corresponding prefabs
    [SerializeField] private GameObject poolContainer; // Parent object to hold the pooled projectiles in the hierarchy
    
    private Dictionary<ProjectileVariant, Queue<GameObject>> projectilePools; // Dictionary to hold the object pools for each projectile variant
    private Dictionary<ProjectileVariant, GameObject> prefabLookup; // Dictionary to quickly look up the prefab for a given projectile variant
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        projectilePools = new Dictionary<ProjectileVariant, Queue<GameObject>>();
        prefabLookup = new Dictionary<ProjectileVariant, GameObject>(); 
        
        // Init pool
        foreach (var pe in projectileEntries)
        {
            Queue<GameObject> pool = new Queue<GameObject>();
            for (int i = 0; i < pe.poolSize; i++)
            {
                GameObject go = Instantiate(pe.prefab, poolContainer.transform);
                go.SetActive(false);
                pool.Enqueue(go);
            }
            projectilePools.Add(pe.variant, pool);
            prefabLookup.Add(pe.variant, pe.prefab);
        }
    }

    public GameObject GetProjectile(ProjectileVariant variant)
    {
        if (projectilePools.TryGetValue(variant, out Queue<GameObject> pool))
        {
            if (pool.Count == 0)
            {
                GameObject go = Instantiate(prefabLookup[variant], poolContainer.transform);
                go.SetActive(false);
                pool.Enqueue(go);
            }
            
            GameObject projectile = pool.Dequeue();
            projectile.SetActive(true);
            return projectile;
        }

        return null;
    }

    public void ReturnProjectile(ProjectileVariant variant, GameObject projectile)
    {
        if (projectilePools.TryGetValue(variant, out Queue<GameObject> pool))
        {
            projectile.SetActive(false);
            projectile.transform.SetParent(poolContainer.transform); // Optional: Reset parent to pool container for organization
            pool.Enqueue(projectile);
        }
        else
        {
            Destroy(projectile); // If no pool exists for this variant, destroy the projectile
        }
    }
    
}