using UnityEngine;

public class GrenadePack : MonoBehaviour, ICollectable
{
    [Header("Grenade Pack Settings")]
    [SerializeField] private ItemType itemType = ItemType.GrenadePack;
    [SerializeField] private int grenadeAmount = 5;
    [SerializeField] private AudioClip[] collectSound;

    public void Collect(InventoryManager inventory)
    {
        inventory.AddItem(ItemType.GrenadePack, grenadeAmount);
        if (collectSound != null && collectSound.Length > 0)
        {
            AudioSource.PlayClipAtPoint(collectSound[Random.Range(0, collectSound.Length)], transform.position);
        }
    }
}
