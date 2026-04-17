using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Staff : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Transform spellGen;
    private Spell spell;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Debug.Log("Mouse Clicked");
            //spell.Cast(spellGen);
        }
    }
}
