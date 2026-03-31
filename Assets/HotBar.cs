using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class HotBar : MonoBehaviour
{
    public Image[] hotbarSlots;
    public Sprite[] itemIcons;
    public GameObject[] items;//NEED TO MAKE AN ITEM CLASS TO HOLD THE ITEMS INSTEAD OF USING GAMEOBJECTS, SO I CAN HAVE METHODS LIKE EQUIP, UNEQUIP, USE, ETC.
    public Transform root;
    public int selectedSlot = 0;
    public Image hotbarSelector;
    private int previousSelectedSlot = 0;

    void Start()
    {
        hotbarSelector.transform.position = hotbarSlots[selectedSlot].transform.position;
        hotbarSlots[0].sprite = itemIcons[0];
        hotbarSlots[1].sprite = itemIcons[1];
        hotbarSlots[2].sprite = itemIcons[2];
        hotbarSlots[3].sprite = itemIcons[3];
    
    }

    // Update is called once per frame
    void Update()
    {
        hotbarSelector.transform.position = hotbarSlots[selectedSlot].transform.position;
        if (previousSelectedSlot != selectedSlot)
        {
            items[previousSelectedSlot].GetComponent<Weapon>().Unequip();
            items[selectedSlot].GetComponent<Weapon>().Equip(root);
            UnityEngine.Debug.Log("Equipped " + items[selectedSlot].name);
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
