using System;
using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class WeaponSwitching : MonoBehaviour
{
    [SerializeField] private int selectedWeapon = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SelectWeapon();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void SelectWeapon()
    {
        int i = 0;
        foreach (Transform weapon in transform)
        {
            if (i == selectedWeapon)
                weapon.gameObject.SetActive(true);
            else
                weapon.gameObject.SetActive(false);
            i++;
        }
    }


    public void OnSelectWeapon(CallbackContext ctx)
    {
        int number = -1;
        if (!ctx.performed && !int.TryParse(ctx.control.name, out number) && number < 0) return;


        int weaponIndex = number - 1;
        if (weaponIndex >= 0 && weaponIndex < transform.childCount)
        {
            selectedWeapon = weaponIndex;
            SelectWeapon();
        }
    }

    public void OnMouseWheel(CallbackContext ctx)
    {
        Debug.Log("Mouse wheel scrolled: " + ctx.ReadValue<float>());
        int previousSelectedWeapon = selectedWeapon;

        if (ctx.ReadValue<float>() > 0)
        {
            selectedWeapon++;
            if (selectedWeapon >= transform.childCount)
                selectedWeapon = 0;
        }
        else if (ctx.ReadValue<float>() < 0)
        {
            selectedWeapon--;
            if (selectedWeapon < 0)
                selectedWeapon = transform.childCount - 1;
        }
        
        if (previousSelectedWeapon != selectedWeapon)
        {
            SelectWeapon();
        }
    }

}
