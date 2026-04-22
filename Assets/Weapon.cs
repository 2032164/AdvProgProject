using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private float damage = 5f;
    [SerializeField] private float knockbackSpeed = 4f;
    [SerializeField] private float knockbackDuration = 0.15f;
    [SerializeField] private float collisionPushSpeed = 6f;
    [SerializeField] private Animator anim;
    [SerializeField] private string attackTriggerName = "Attack";
    [SerializeField] private string swingStateName = "SwordSwing";

    // Make this true if your weapon collider is intended to be a trigger.
    [SerializeField] private bool forceTrigger = false;
    public Sprite weaponIcon;
    private GameObject thisWeapon;

    void Start()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = forceTrigger;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (anim != null)
            {
                anim.SetTrigger(attackTriggerName);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        TryPushRigidbody(other);
        TryHitEnemy(other.transform.root);
    }

    void OnCollisionEnter(Collision hit)
    {
        TryPushRigidbody(hit.collider);
        TryHitEnemy(hit.transform.root);
    }

    private void TryPushRigidbody(Collider hitCollider)
    {
        if (!IsSwinging())
        {
            return;
        }

        Rigidbody body = hitCollider.attachedRigidbody;
        if (body == null || body.isKinematic)
        {
            return;
        }

        Vector3 pushDir = body.worldCenterOfMass - transform.position;
        pushDir.y = 0f;
        if (pushDir.sqrMagnitude < 0.0001f)
        {
            return;
        }

        body.AddForce(pushDir.normalized * collisionPushSpeed, ForceMode.VelocityChange);
    }

    private void TryHitEnemy(Transform root)
    {
        if(!IsSwinging())
        {
            return;
        }
        Enemy enemy = root.GetComponent<Enemy>();
        if (enemy == null)
            return;

        enemy.TakeDamage(damage);
        enemy.KnockbackFrom(transform, knockbackSpeed, knockbackDuration);
        UnityEngine.Debug.Log("Weapon hit " + enemy.name);
    }

    private bool IsSwinging()
    {
        if (anim == null)
        {
            return false;
        }

        AnimatorStateInfo current = anim.GetCurrentAnimatorStateInfo(0);
        if (current.IsName(swingStateName))
        {
            return true;
        }

        if (anim.IsInTransition(0))
        {
            AnimatorStateInfo next = anim.GetNextAnimatorStateInfo(0);
            return next.IsName(swingStateName);
        }

        return false;
    }

    public void Equip(Transform root)
    {
        thisWeapon = Instantiate(this.gameObject, root);
        transform.SetParent(root);
    }

    public void Unequip()
    {
        Destroy(thisWeapon);
    }

}
