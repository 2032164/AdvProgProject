using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;

public class Spell : MonoBehaviour
{
    // Start is called before the first frame update
    public int rarity;//each will correspond to a different spell type and top gem color [grey,green,blue,purple,orange]
    public Color[] spellColors = new Color[5];//each will correspond to a different spell type and top gem color [grey,green,blue,purple,orange]
    public int spellType;//each will correspond to a different spell effect and bottom gem color
    void Start()
    {
        //need to make it so that first time it gets cast it creates a random spell and then after that it just casts the spell
        rarity = Random.Range(1, 6); // Assuming rarity values are 1-5
        spellType = Random.Range(1, 6); // Assuming spellType values are 1-5

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
