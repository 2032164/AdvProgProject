using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private float damage = 5f;
    [SerializeField] private float knockbackSpeed = 4f;
    [SerializeField] private float knockbackDuration = 0.15f;

    // Make this true if your weapon collider is intended to be a trigger.
    [SerializeField] private bool forceTrigger = true;

    void Start()
    {
        Collider col = GetComponent<Collider>();
        if (col != null && forceTrigger)
        {
            col.isTrigger = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider other)
    {
        TryHitEnemy(other.transform.root);
    }

    void OnCollisionEnter(Collision hit)
    {
        TryHitEnemy(hit.transform.root);
    }

    private void TryHitEnemy(Transform root)
    {
        Enemy enemy = root.GetComponent<Enemy>();
        if (enemy == null)
            return;

        enemy.TakeDamage(damage);
        enemy.KnockbackFrom(transform, knockbackSpeed, knockbackDuration);
        UnityEngine.Debug.Log("Weapon hit " + enemy.name);
    }

}
