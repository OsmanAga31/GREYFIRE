using UnityEngine;

// public enum WeaponIndex
// {
//     Rifle,
//     SpellShot,
//     Shotgun
// }

public class Inventory : MonoBehaviour
{
    [SerializeField] private GameObject[] inventory;
    [SerializeField] private int[] ammo;
}