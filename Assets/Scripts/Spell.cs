// Spell descriptor,has rarity/type and casts prefab.

using UnityEngine;
using Color = UnityEngine.Color;

public class Spell
{
    public Color rarity;//each will correspond to a different spell type and top gem color [grey,green,blue,purple,orange]
    private Color[] rarityColors = {Color.grey,Color.green,Color.blue,Color.magenta,new Color(1f, 0.647f, 0f)};//each will correspond to a different spell type and top gem color [grey,green,blue,purple,orange]
    public Color spellType;//each will correspond to a different spell effect and bottom gem color
    private Color[] typeColors = {Color.red,new Color(0.4f, 0.2f, 0.1f), new Color(0f, 0.5f, 0.5f),Color.grey};//fire,earth,water,wind
    private int spellTypeIndex = 0;

    public double cooldown = .5;
    private double lastCastTime = -9999;
    public GameObject[] spellPrefabs;//fireball,earthsmash,waterbeam,
    private float multiplier = 1;

    public Spell(int rarity,int spellType, GameObject[] spellPrefabs)
    {
        //makes it so that first time it gets cast it creates a random spell and then after that it just casts the spell
        this.rarity = rarityColors[rarity];
        multiplier = 1 + (rarity * 0.1f );
        this.spellType = typeColors[spellType];
        this.spellTypeIndex = spellType;
        this.spellPrefabs = spellPrefabs;
    }

    public void cast(Transform spellGen, Vector3 direction)
    {
        if (spellPrefabs == null || spellPrefabs.Length == 0)
        {
            UnityEngine.Debug.LogWarning("No spell prefabs assigned to Spell.");
            return;
        }

        int index = Mathf.Clamp(spellTypeIndex, 0, spellPrefabs.Length - 1);
        GameObject spell = UnityEngine.Object.Instantiate(spellPrefabs[index], spellGen.position, spellGen.rotation);

        FireBall fire = spell.GetComponent<FireBall>();
        if (fire == null) fire = spell.GetComponentInChildren<FireBall>();
        if (fire != null)
        {
            fire.SetDirection(direction);
            return;
        }

        WaterSlash water = spell.GetComponent<WaterSlash>();
        if (water == null) water = spell.GetComponentInChildren<WaterSlash>();
        if (water != null)
        {
            water.SetDirection(direction);
            return;
        }

        RockSmash rock = spell.GetComponent<RockSmash>();
        if (rock == null) rock = spell.GetComponentInChildren<RockSmash>();
        if (rock != null)
        {
            rock.SetDirection(direction);
            return;
        }


    }
}
