using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public bool healing;
    public float maxHealth;
    public GameObject player;
    private float healAmount = .01f;
    // Start is called before the first frame update
    [SerializeField]
    private Image healthBarFill;
    void Start()
    {
        //need to figure out how to make the fill amount scale to the right full health if that makes sense
        healthBarFill.fillAmount = player.GetComponent<FPSController>().health / maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        healthBarFill.fillAmount = player.GetComponent<FPSController>().getHealth() / maxHealth;
        UnityEngine.Debug.Log($"Player health: {player.GetComponent<FPSController>().getHealth()}");
        UnityEngine.Debug.Log($"Health bar fill amount: {healthBarFill.fillAmount}");
        //if(healthBarFill.fill < maxHealth){
        //    healthBarFill.fill += healAmount;
        //}

    }
}
