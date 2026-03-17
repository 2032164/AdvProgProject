using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public bool healing;
    public float maxHealth;
    private bool healAmount = .01f;
    // Start is called before the first frame update
    [SerializeField]
    private Image healthBarFill;
    void Start()
    {
        //need to figure out how to make the fill amount scale to the right full health if that makes sense
        healthBarFill.fillAmount = .5F;
    }

    // Update is called once per frame
    void Update()
    {
        healthBarFill.fillAmount += healAmount;
    }
}
