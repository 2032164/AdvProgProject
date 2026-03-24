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

    private FPSController fpsController;
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

    // Update is called once per frame
    void Update()
    {
        float currentHealth = fpsController.getHealth();

        if (currentHealth < previousHealth)
        {
            lastDamageTime = Time.time;
        }

        bool cooldownComplete = Time.time - lastDamageTime >= healDelay;
        if (cooldownComplete && currentHealth < maxHealth)
        {
            fpsController.health = Mathf.Min(maxHealth, fpsController.health + (healAmount * Time.deltaTime));
            currentHealth = fpsController.health;
        }
        else
        {
        }
        healthBarFill.fillAmount = Mathf.Clamp01(currentHealth / maxHealth);
        previousHealth = currentHealth;
    }
}
