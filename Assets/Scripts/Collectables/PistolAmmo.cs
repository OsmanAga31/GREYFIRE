using UnityEngine;

public class PistolAmmo : MonoBehaviour, ICollectable
{
    [Header("Pistol Ammo Settings")]
    [SerializeField] private AmmoType itemType = AmmoType.PistolAmmo;
    [SerializeField] private int ammoAmount = 15;
    [SerializeField] private AudioClip[] collectSound;

    public void Collect(InventoryManager inventory)
    {
        inventory.AddItem(AmmoType.PistolAmmo, ammoAmount);
        if (collectSound != null && collectSound.Length > 0)
        {
            AudioSource.PlayClipAtPoint(collectSound[Random.Range(0, collectSound.Length)], transform.position);
        }
    }
}
