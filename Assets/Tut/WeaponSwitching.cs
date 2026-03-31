using System;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class WeaponSwitching : MonoBehaviour
{
    [SerializeField] private int selectedWeapon = 0;
    [SerializeField] private PlayerInput playerInput;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SelectWeapon();
    }

    private void OnEnable()
    {
        playerInput.actions["WeaponSelect"].performed += OnSelectWeapon;
        playerInput.actions["Scroll"].performed += OnMouseWheel;
    }

    private void OnDisable()
    {
        playerInput.actions["WeaponSelect"].performed -= OnSelectWeapon;
        playerInput.actions["Scroll"].performed -= OnMouseWheel;
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


    public void OnSelectWeapon(CallbackContext ctx) // fixed with help of A.I. Claude - No Copy-Paste but look, understand and adapt
    {
        var number = 0;
        if (!int.TryParse(ctx.control.name, out number)) return;

        int weaponIndex = (number == 0) ? 9 : number - 1;
        if (weaponIndex >= 0 && weaponIndex < transform.childCount)
        {
            selectedWeapon = weaponIndex;
            SelectWeapon();
        }
    }

    public void OnMouseWheel(CallbackContext ctx)
    {
        var scrollValue = ctx.ReadValue<Vector2>().y;
        if (scrollValue > 0)
        {
            selectedWeapon = (selectedWeapon + 1) % transform.childCount;
            SelectWeapon();
        }
        else if (scrollValue < 0)
        {
            selectedWeapon = (selectedWeapon - 1 + transform.childCount) % transform.childCount;
            SelectWeapon();
        }

    }

}
