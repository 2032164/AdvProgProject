using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Color = UnityEngine.Color;

public class Staff : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Transform spellGen;
    public GameObject topGem;
    public GameObject bottomGem;
    private Spell spell;
    public GameObject[] spellPrefabs;//fireball,earthsmash?,waterbeam,wind?
    void Start()//THERES A PROBLEM WITH THE STAFF BECAUSE EACH TIME U SELECT IT IN HOTBAR IT REGENS BC THE OLD ONE GETS DESTROYED AND A NEW ONE GETS INSTANTIATED
    {
        RollSpell();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse Clicked");
            //RollSpell();
            spell.cast(spellGen, transform.forward);
        }
    }

    private void RollSpell()
    {
        spell = new Spell(Random.Range(0,5),Random.Range(0,4), spellPrefabs);//need to change it so higher rarities are rarer
        SetGemColor(topGem, spell.rarity);
        SetGemColor(bottomGem, spell.spellType);
    }

    private void SetGemColor(GameObject gem, Color color)
    {
        Material material = gem.GetComponent<Renderer>().material;
        material.color = color;

        // Keep emission synchronized with the visible gem color.
        material.EnableKeyword("_EMISSION");
        material.SetColor("_EmissionColor", color);
    }
    
}
