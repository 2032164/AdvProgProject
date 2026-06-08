// Projectile for water spells: lifetime, movement, rotation, and collisions.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class WaterSlash : MonoBehaviour
{
    public float timeout = 2f; // Destroy after 5 seconds
    public float speed = 7.5f; // Speed of the water slash
    public float rotationSpeed = 360f;
    private float elapsedTime = 0f;
    private Vector3 direction;
    private Rigidbody rb;
    private bool directionSet = false;
    public float damage = 30f; //
    public float knockbackSpeed = 0f;
    public float knockbackDuration = 0f;
    public GameObject sphere;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = false;
        }
    }

    // Set the direction for the water slash to travel
    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection.normalized;
        if (sphere != null)
        {
            sphere.transform.rotation = Quaternion.LookRotation(direction);
            sphere.transform.Rotate(90f, 0f, 0f);


        }
        directionSet = true;
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= timeout)
        {
            DestroyWaterSlash();
        }
        
        // Move the water slash forward
        if (directionSet)
        {
            transform.Translate(direction * speed * Time.deltaTime, Space.World);
            transform.Rotate(0f, rotationSpeed * Time.deltaTime, 0f, Space.Self);
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        UnityEngine.Debug.Log("WaterSlash collided with: " + collision.gameObject.name);
        TryPushRigidbody(collision.collider);
        TryHitEnemy(collision.transform.root);
        if (collision.gameObject.CompareTag("Wall"))
        {
            UnityEngine.Debug.Log("Collided with wall, destroying WaterSlash");
            DestroyWaterSlash();
        }
    }

    // Try to hit an enemy (similar to Weapon class pattern)
    private void TryHitEnemy(Transform root)
    {
        UnityEngine.Debug.Log("Trying to hit enemy");
        Enemy enemy = root.GetComponent<Enemy>();
        if (enemy == null)
            return;
        UnityEngine.Debug.Log("Hit enemy for " + damage);
        enemy.TakeDamage(damage);
        enemy.KnockbackFrom(transform, knockbackSpeed, knockbackDuration);
        UnityEngine.Debug.Log("Weapon hit " + enemy.name);
    }

    private void TryPushRigidbody(Collider hitCollider)
    {
        UnityEngine.Debug.Log("Trying to push rigidbody");
        Rigidbody body = hitCollider.attachedRigidbody;
        if (body == null || body.isKinematic)
        {
            UnityEngine.Debug.Log("Trying to push rigidbody but it has no rigidbody or is kinematic");
            return;
        }

        Vector3 pushDir = body.worldCenterOfMass - transform.position;
        pushDir.y = 0f;
        if (pushDir.sqrMagnitude < 0.0001f)
        {
            UnityEngine.Debug.Log("Trying to push rigidbody but push direction is too small");
            return;
        }
        UnityEngine.Debug.Log("Pushing rigidbody");
        body.AddForce(pushDir.normalized * knockbackSpeed, ForceMode.VelocityChange);
    }
    // Destroy water slash and its parent
    private void DestroyWaterSlash()
    {
        UnityEngine.Debug.Log("Destroying WaterSlash");
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
