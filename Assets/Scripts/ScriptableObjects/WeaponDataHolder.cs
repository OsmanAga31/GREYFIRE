using UnityEngine;

public class WeaponDataHolder : MonoBehaviour
{
    [SerializeField] private WeaponData wd40;

    private WeaponData WeaponData => wd40;

}
