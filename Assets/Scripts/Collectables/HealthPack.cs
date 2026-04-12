using UnityEngine;

public class HealthPack : MonoBehaviour, ICollectable
{
    [Header("Health Pack Settings")]
    [SerializeField] private AmmoType itemType = AmmoType.HealthPack;
    [SerializeField] private int healthAmount = 25;
    [SerializeField] private AudioClip[] collectSound;

    public void Collect(InventoryManager inventory)
    {
        inventory.AddItem(AmmoType.HealthPack, healthAmount);
        if (collectSound != null && collectSound.Length > 0)
        {
            AudioSource.PlayClipAtPoint(collectSound[Random.Range(0, collectSound.Length)], transform.position);
        }
    }
}
