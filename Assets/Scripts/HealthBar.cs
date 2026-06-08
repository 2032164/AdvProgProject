// Player health manager, handles passive healing and campfire-boosted healing.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public float maxHealth;
    public GameObject player;

    [SerializeField]
    private float healDelay = 3f;

    [SerializeField]
    private float healAmount = 10f;

    [SerializeField]
    private float campfireHealAmount = 20f;

    private FPSController fpsController;
    private bool isInCampfire = false;
    private float previousHealth;
    private float lastDamageTime;

    // Start is called before the first frame update
    [SerializeField]
    private Image healthBarFill;

    void Start()
    {
        fpsController = player.GetComponent<FPSController>();
        previousHealth = fpsController.getHealth();
        lastDamageTime = Time.time;

        healthBarFill.fillAmount = Mathf.Clamp01(previousHealth / maxHealth);
    }

    public void SetCampfireHealing(bool active)
    {
        isInCampfire = active;
        UnityEngine.Debug.Log("Campfire healing set to: " + active);
    }

    // Update is called once per frame
    void Update()
    {
        float currentHealth = fpsController.getHealth();

        if (currentHealth < previousHealth)
        {
            lastDamageTime = Time.time;
        }

        float activeHealAmount = isInCampfire ? campfireHealAmount : healAmount;
        bool cooldownComplete = Time.time - lastDamageTime >= healDelay;
        
        if ((isInCampfire || cooldownComplete) && currentHealth < maxHealth)
        {
            UnityEngine.Debug.Log("Healing player faster "+ isInCampfire);
            fpsController.health = Mathf.Min(maxHealth, fpsController.health + (activeHealAmount * Time.deltaTime));
            currentHealth = fpsController.health;
        }
        healthBarFill.fillAmount = Mathf.Clamp01(currentHealth / maxHealth);
        previousHealth = currentHealth;
    }
}
