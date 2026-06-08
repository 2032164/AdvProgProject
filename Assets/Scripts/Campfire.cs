using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security;
using UnityEngine;
public class AreaDetection : MonoBehaviour
{
    // This is called automatically by Unity when another collider enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        // Use CompareTag for better performance and to avoid typos
        UnityEngine.Debug.Log("Collider entered: " + other.name);
        if (other.CompareTag("Player"))
        {
            UnityEngine.Debug.Log("Player has entered the campfire area");
            FPSController fpsController = other.GetComponent<FPSController>();
            HealthBar healthBar = null;
            if (fpsController != null && fpsController.healthBar != null)
            {
                healthBar = fpsController.healthBar.GetComponent<HealthBar>();
            }
            if (healthBar != null)
            {
                healthBar.SetCampfireHealing(true); // Start faster healing
                UnityEngine.Debug.Log("Player is now healing faster.");
            }
            if (fpsController != null)
            {
                fpsController.enterUpgradeMenu();
            }
        }
    }

    // Optional: Detect when the player leaves the area
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FPSController fpsController = other.GetComponent<FPSController>();
            HealthBar healthBar = null;
            if (fpsController != null && fpsController.healthBar != null)
            {
                healthBar = fpsController.healthBar.GetComponent<HealthBar>();
            }
            if (healthBar != null)
            {
                healthBar.SetCampfireHealing(false); // Stop faster healing
            }
            UnityEngine.Debug.Log("Player left the area.");
        }
    }
}

