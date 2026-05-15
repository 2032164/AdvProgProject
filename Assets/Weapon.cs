using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] private float damage = 5f;
    [SerializeField] private float knockbackSpeed = 4f;
    [SerializeField] private float knockbackDuration = 0.15f;
    [SerializeField] private float collisionPushSpeed = 6f;
    [SerializeField] private Animator anim;
    [SerializeField] private string attackTriggerName = "Attack";
    [SerializeField] private bool forceTrigger = false;
    [SerializeField] private Transform unequippedParent;
    public Sprite weaponIcon;
    private bool started = false;
    private GameObject thisWeapon;

    void Start()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = forceTrigger;
        }
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if (anim != null && !AnimatorIsPlaying(attackTriggerName))
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
        body.AddForce(pushDir.normalized * collisionPushSpeed, ForceMode.VelocityChange);
    }

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

    public void Equip(Transform root)
    {
        if (root == null)
        {
            UnityEngine.Debug.LogWarning("Equip called with null root.");
            return;
        }

        if(!started){
            thisWeapon = Instantiate(this.gameObject, root);
            thisWeapon.transform.SetParent(root, false);
            thisWeapon.SetActive(true);
            started = true;
        }
        else{
            if (thisWeapon == null)
            {
                thisWeapon = Instantiate(this.gameObject, root);
            }

            thisWeapon.transform.SetParent(root, false);
            thisWeapon.SetActive(true);
        }
    }

    public void Unequip()
    {
        if (!started || thisWeapon == null)
        {
            return;
        }

        if (unequippedParent != null)
        {
            thisWeapon.transform.SetParent(unequippedParent, false);
        }

        thisWeapon.SetActive(false);
    }

    bool AnimatorIsPlaying(){
        return anim.GetCurrentAnimatorStateInfo(0).length > anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
    bool AnimatorIsPlaying(string stateName){
        return AnimatorIsPlaying() && anim.GetCurrentAnimatorStateInfo(0).IsName(stateName);
    }

}
