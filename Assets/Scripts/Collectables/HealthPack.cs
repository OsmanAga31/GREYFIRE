using UnityEngine;

public class HealthPack : MonoBehaviour, ICollectable
{
    [Header("Health Pack Settings")]
    [SerializeField] private ItemType itemType = ItemType.HealthPack;
    [SerializeField] private int healthAmount = 25;
    [SerializeField] private AudioClip[] collectSound;

    public void Collect(InventoryManager inventory)
    {
        inventory.AddItem(ItemType.HealthPack, healthAmount);
        if (collectSound != null && collectSound.Length > 0)
        {
            AudioSource.PlayClipAtPoint(collectSound[Random.Range(0, collectSound.Length)], transform.position);
        }
    }
}
