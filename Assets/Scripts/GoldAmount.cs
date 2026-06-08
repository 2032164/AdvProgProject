// Tracks player's gold and updates the UI text element.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoldAmount : MonoBehaviour
{
    // Start is called before the first frame update
    public TextMeshProUGUI goldText;
    private int gold = 0;
    void Start()
    {
        goldText.text = "Gold: " + gold;
    }

    // Update is called once per frame
    public void AddGold(int amount)
    {
        gold += amount;
        UpdateGoldText();
    }

    public bool RemoveGold(int amount)
    {
        if (gold >= amount)
        {
            gold = gold - amount;
            UpdateGoldText();
            return true;
        }
        return false;
    }

    void UpdateGoldText()
    {
        goldText.text = "Gold: " + gold;
    }
}
