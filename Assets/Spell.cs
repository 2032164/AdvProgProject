using UnityEngine;
using Color = UnityEngine.Color;

public class Spell
{
    public Color rarity;//each will correspond to a different spell type and top gem color [grey,green,blue,purple,orange]
    private Color[] rarityColors = {Color.grey,Color.green,Color.blue,Color.magenta,new Color(1f, 0.647f, 0f)};//each will correspond to a different spell type and top gem color [grey,green,blue,purple,orange]
    public Color spellType;//each will correspond to a different spell effect and bottom gem color
    private Color[] typeColors = {Color.red,new Color(0.4f, 0.2f, 0.1f), new Color(0f, 0.5f, 0.5f),Color.grey};//fire,earth,water,wind?
    public Spell(int rarity,int spellType)
    {
        //need to make it so that first time it gets cast it creates a random spell and then after that it just casts the spell
        this.rarity = rarityColors[rarity];
        this.spellType = typeColors[spellType];
    }
}
