using UnityEngine;

public class RifleAmmo : MonoBehaviour, ICollectable
{
    [Header("Rifle Ammo Settings")]
    [SerializeField] private ItemType itemType = ItemType.RifleAmmo;
    [SerializeField] private int ammoAmount = 30;
    [SerializeField] private AudioClip[] collectSound;
    public void Collect(InventoryManager inventory)
    {
        inventory.AddItem(ItemType.RifleAmmo, ammoAmount);
        if (collectSound != null && collectSound.Length > 0)
        {
            AudioSource.PlayClipAtPoint(collectSound[Random.Range(0, collectSound.Length)], transform.position);
        }
    }
}
