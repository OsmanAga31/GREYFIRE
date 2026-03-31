using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private GameObject[] items;

    public void AddItem(ItemType itemType, int amount)
    {
        items[(int)itemType].GetComponent<IItemAdder>().Add(amount);
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Collectable"))
        {
            ICollectable collectable = collision.gameObject.GetComponent<ICollectable>();
            if (collectable != null)
            {
                collectable.Collect(this);
                Destroy(collision.gameObject);
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