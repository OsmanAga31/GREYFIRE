using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Combat/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName; // Name of the weapon
    public DamageType damageType;
    public float baseDamage; // Base damage of the weapon
    public float fireRate; // Time between shots in seconds
    public GameObject projectilePrefab; // Prefab for the projectile fired by the weapon
    public SpreadSettings spreadSettings; // Settings for spread
    public AOESettings aoeSettings; // Settings for area of effect damage
}
