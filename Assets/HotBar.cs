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

    void Start()
    {
        hotbarSelector.transform.position = hotbarSlots[selectedSlot].transform.position;
        hotbarSlots[0].sprite = items[0].GetComponent<Weapon>().weaponIcon;
        hotbarSlots[1].sprite = items[1].GetComponent<Weapon>().weaponIcon;
        hotbarSlots[2].sprite = items[2].GetComponent<Weapon>().weaponIcon;
        hotbarSlots[3].sprite = items[3].GetComponent<Weapon>().weaponIcon;
    }

    // Update is called once per frame
    void Update()
    {
        if(previousSelectedSlot == -1)
        {
            items[0].GetComponent<Weapon>().Equip(root);
            previousSelectedSlot = 0;
        }
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
            items[previousSelectedSlot].GetComponent<Weapon>().Unequip();
            items[selectedSlot].GetComponent<Weapon>().Equip(root);
            previousSelectedSlot = selectedSlot;
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
}
