using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    //[SerializeField] private CharacterController characterController;
    [SerializeField] private GameObject[] items;

    public void AddItem(AmmoType itemType, int amount)
    {
        if (itemType == AmmoType.None || (int)itemType >= items.Length)
        {
            Debug.LogWarning("Invalid item type: " + itemType);
            return;
        }
        items[(int)itemType].GetComponent<IItemAdder>().Add(amount, itemType);
    }

    public void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Collectable"))
        {
            ICollectable collectable = hit.gameObject.GetComponent<ICollectable>();
            if (collectable != null)
            {
                collectable.Collect(this);
                Destroy(hit.gameObject);
            }
        }
    }

}
public enum AmmoType
{
    None,
    HealthPack,
    RifleAmmo,
    SpellShotAmmo,
    GrenadePack,
    PistolAmmo
}