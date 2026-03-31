using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private CharacterController characterController;
    [SerializeField] private GameObject[] items;

    public void AddItem(ItemType itemType, int amount)
    {
        items[(int)itemType].GetComponent<IItemAdder>().Add(amount);
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
public enum ItemType
{
    None,
    HealthPack,
    RifleAmmo,
    PistolAmmo,
    GrenadePack
}