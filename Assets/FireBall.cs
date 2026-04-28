using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    public float timeout = 5f; // Destroy after 5 seconds
    public float speed = 10f; // Speed of the fireball
    private float elapsedTime = 0f;
    private Vector3 direction;
    private Rigidbody rb;
    private bool directionSet = false;
    public float damage = 5f; // Base damage, can be modified based on spell type and rarity
    public float knockbackSpeed = 1f;
    public float knockbackDuration = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
        }
    }

    // Set the direction for the fireball to travel
    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection.normalized;
        directionSet = true;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= timeout)
        {
            DestroyFireball();
        }
        
        // Move the fireball forward
        if (directionSet)
        {
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        UnityEngine.Debug.Log("Fireball collided with: " + collision.gameObject.name);
        TryHitEnemy(collision.transform);
        DestroyFireball();
    }

    // Try to hit an enemy (similar to Weapon class pattern)
    private void TryHitEnemy(Transform root)
    {
        UnityEngine.Debug.Log("Trying to hit enemy");
        Enemy enemy = root.GetComponent<Enemy>();
        if (enemy == null)
            return;
        UnityEngine.Debug.Log("Hit enemy");
        enemy.TakeDamage(damage);
        enemy.KnockbackFrom(transform, knockbackSpeed, knockbackDuration);
        UnityEngine.Debug.Log("Weapon hit " + enemy.name);
    }

    // Destroy fireball and its parent
    private void DestroyFireball()
    {
        UnityEngine.Debug.Log("Destroying fireball");
        if (transform.parent != null)
        {
            Destroy(transform.parent.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
