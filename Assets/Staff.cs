using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Staff : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Transform spellGen;
    public gameObject topGem;
    public gameObject bottomGem;
    private Spell spell;
    void Start()
    {
        spell = new Spell(Random.range(0,5),Random.range(0,4));//need to change it so higher rarities are rarer
        topGem.GetComponent<Renderer>().material.color = spell.topGem;
        bottomGem.GetComponent<Renderer>().material.color = spell.bottomGem; 
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse Clicked");
            spell = new Spell(Random.range(0,5),Random.range(0,4));//need to change it so higher rarities are rarer
            topGem.GetComponent<Renderer>().material.color = spell.topGem;
            bottomGem.GetComponent<Renderer>().material.color = spell.bottomGem; 

        }
    }
    
}
