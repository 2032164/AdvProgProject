using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class HotBar : MonoBehaviour
{
    public Image[] hotbarSlots;
    public int selectedSlot = 0;
    public Image hotbarSelector;
    void Start()
    {
        hotbarSelector.transform.position = hotbarSlots[selectedSlot].transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        hotbarSelector.transform.position = hotbarSlots[selectedSlot].transform.position;
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
