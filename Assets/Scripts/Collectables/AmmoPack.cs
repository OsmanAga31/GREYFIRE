using UnityEngine;

public class AmmoPack : MonoBehaviour, ICollectable
{
    [Header("Ammo Settings")]
    [SerializeField] private AmmoType itemType = AmmoType.RifleAmmo;
    [SerializeField] private int collectAmount = 30;
    [SerializeField] private AudioClip[] collectSound;
    public void Collect(InventoryManager inventory)
    {
        inventory.AddItem(itemType, collectAmount);
        if (collectSound != null && collectSound.Length > 0)
        {
            AudioSource.PlayClipAtPoint(collectSound[Random.Range(0, collectSound.Length)], transform.position);
        }
    }
}
