// UI hotbar manager, fills slots, handles selection input, and equips items, manages re-rolling items.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class HotBar : MonoBehaviour
{
    public Image[] hotbarSlots;
    public GameObject[] items;
    public Transform root;
    public int selectedSlot = 0;
    public Image hotbarSelector;
    private int previousSelectedSlot = -1;
    private GameObject[] equippedWeapons;

    void Start()
    {
        PopulateHotbarWithRandomWeapons();
        hotbarSelector.transform.position = hotbarSlots[selectedSlot].transform.position;
        EquipCurrentSlot();
    }


    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedSlot = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedSlot = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedSlot = 2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectedSlot = 3;
        }
        hotbarSelector.transform.position = hotbarSlots[selectedSlot].transform.position;
        if (previousSelectedSlot != selectedSlot && previousSelectedSlot != -1)
        {
            UnequipSlot(previousSelectedSlot);
            EquipCurrentSlot();
        }
    }

    public void OnScroll()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll < 0f)
        {
            selectedSlot = (selectedSlot + 1) % hotbarSlots.Length;
        }
        else if (scroll > 0f)
        {
            selectedSlot = (selectedSlot - 1 + hotbarSlots.Length) % hotbarSlots.Length;
        }
    }

    private void PopulateHotbarWithRandomWeapons()
    {
        equippedWeapons = new GameObject[hotbarSlots.Length];

        List<GameObject> availableWeapons = new List<GameObject>();
        foreach (GameObject item in items)
        {
            if (item != null)
            {
                availableWeapons.Add(item);
            }
        }

        for (int slotIndex = 0; slotIndex < hotbarSlots.Length; slotIndex++)
        {
            if (availableWeapons.Count == 0)
            {
                hotbarSlots[slotIndex].sprite = null;
                hotbarSlots[slotIndex].enabled = false;
                equippedWeapons[slotIndex] = null;
                continue;
            }

            int weaponIndex = Random.Range(0, availableWeapons.Count);
            GameObject weaponObject = availableWeapons[weaponIndex];
            availableWeapons.RemoveAt(weaponIndex);

            equippedWeapons[slotIndex] = weaponObject;
            Weapon weapon = weaponObject.GetComponent<Weapon>();
            hotbarSlots[slotIndex].sprite = weapon != null ? weapon.weaponIcon : null;
            hotbarSlots[slotIndex].enabled = true;
        }

        selectedSlot = Mathf.Clamp(selectedSlot, 0, hotbarSlots.Length - 1);
    }

    private void EquipCurrentSlot()
    {
        if (equippedWeapons == null || equippedWeapons.Length == 0)
        {
            return;
        }

        GameObject currentWeapon = equippedWeapons[selectedSlot];
        if (currentWeapon == null)
        {
            previousSelectedSlot = selectedSlot;
            return;
        }

        Weapon weapon = currentWeapon.GetComponent<Weapon>();
        if (weapon == null)
        {
            previousSelectedSlot = selectedSlot;
            return;
        }

        if (previousSelectedSlot >= 0 && previousSelectedSlot < equippedWeapons.Length)
        {
            UnequipSlot(previousSelectedSlot);
        }

        weapon.Equip(root);
        previousSelectedSlot = selectedSlot;
    }

    private void UnequipSlot(int slotIndex)
    {
        if (equippedWeapons == null || slotIndex < 0 || slotIndex >= equippedWeapons.Length)
        {
            return;
        }

        GameObject weaponObject = equippedWeapons[slotIndex];
        if (weaponObject == null)
        {
            return;
        }

        Weapon weapon = weaponObject.GetComponent<Weapon>();
        if (weapon != null)
        {
            weapon.Unequip();
        }
    }

    // Rerolls the requested slot, replacing the equipped weapon with a new random
    // one from the available items while excluding weapons already equipped in
    // other slots and the previous weapon in the same slot.
    public void RerollSlot(int slotIndex)
    {
        if (equippedWeapons == null || equippedWeapons.Length == 0)
        {
            return;
        }

        if (slotIndex < 0 || slotIndex >= equippedWeapons.Length)
        {
            return;
        }

        GameObject currentWeapon = equippedWeapons[slotIndex];
        if (currentWeapon != null)
        {
            Weapon weapon = currentWeapon.GetComponent<Weapon>();
            if (weapon != null)
            {
                weapon.Unequip();
            }
        }

        List<GameObject> availableWeapons = new List<GameObject>();
        foreach (GameObject item in items)
        {
            if (item == null || item.GetComponent<Weapon>() == null)
            {
                continue;
            }

            bool alreadyEquippedElsewhere = false;
            for (int i = 0; i < equippedWeapons.Length; i++)
            {
                if (i == slotIndex)
                {
                    continue;
                }

                if (equippedWeapons[i] == item)
                {
                    alreadyEquippedElsewhere = true;
                    break;
                }
            }

            if (!alreadyEquippedElsewhere)
            {
                availableWeapons.Add(item);
            }
        }

        if (availableWeapons.Count == 0)
        {
            hotbarSlots[slotIndex].sprite = null;
            hotbarSlots[slotIndex].enabled = false;
            equippedWeapons[slotIndex] = null;
            return;
        }

        int weaponIndex = Random.Range(0, availableWeapons.Count);
        GameObject newWeaponObject = availableWeapons[weaponIndex];
        equippedWeapons[slotIndex] = newWeaponObject;

        Weapon newWeapon = newWeaponObject.GetComponent<Weapon>();
        hotbarSlots[slotIndex].sprite = newWeapon != null ? newWeapon.weaponIcon : null;
        hotbarSlots[slotIndex].enabled = true;

        EquipCurrentSlot();
    }
}
