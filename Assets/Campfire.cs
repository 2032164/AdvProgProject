using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AreaDetection : MonoBehaviour
{
    // This is called automatically by Unity when another collider enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        // Use CompareTag for better performance and to avoid typos
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player has entered the campfire area");
            // Insert logic here (e.g., start a cutscene, spawn enemies, or play music)
        }
    }

    // Optional: Detect when the player leaves the area
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player left the area.");
        }
    }
}

