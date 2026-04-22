using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using Color = UnityEngine.Color;

public class Spell : MonoBehaviour
{
    // Start is called before the first frame update
    public int rarity;//each will correspond to a different spell type and top gem color [grey,green,blue,purple,orange]
    public Color[] rarityColors = {Color.grey,Color.green,Color.blue,Color.purple,Color.orange};//each will correspond to a different spell type and top gem color [grey,green,blue,purple,orange]
    public int spellType;//each will correspond to a different spell effect and bottom gem color
    public Color[] typeColors = {Color.red,Color.brown, Color.aquamarine,Color.grey};//fire,earth,water,wind?
    void Start(int rarity,int spellType)
    {
        //need to make it so that first time it gets cast it creates a random spell and then after that it just casts the spell
        this.rarity = rarity;
        this.spellType = spellType;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public Color topGem(){
        return rarityColors[rarity];
    }
    public Color bottomGem(){
        return typeColors[spellType];
    }
}
