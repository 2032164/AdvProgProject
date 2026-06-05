using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.InteropServices;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;


public class Enemy : MonoBehaviour
{
    public float speed = 5f;
    public float activeDistance = 10f;
    public float lostIntrestDistance = 15f;
    public Transform player;
    public GameObject playerBody;
    public float maxHealth = 100f;
    public float health = 100f;
    public float damage = 10f;
    private bool iFrame = false;
    UnityEngine.AI.NavMeshAgent agent;
    Vector3 spawn;
    bool isChasing = false;

    void Start()
    {
       agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
       spawn = transform.position;
    }

    void Update()
    {
        if(health <=0){
            Destroy(gameObject);
        }
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if(!isChasing && distanceToPlayer <= activeDistance)
        {
            isChasing = true;
        }
        if(isChasing && distanceToPlayer >= lostIntrestDistance)
        {
            isChasing = false;
            agent.SetDestination(spawn);
        }

        if(isChasing)
        {
            agent.SetDestination(player.position);
        }

    }
    public void OnPlayerBumped(GameObject player, ControllerColliderHit hit)
    {
        TakeDamage(5f);
    }

    public void TakeDamage(float dmg)
    {
        if (iFrame) return;
        health -= dmg;
        Debug.Log($"{name} health now {health}");

        if (health <= 0f){
            playerBody = GameObject.FindWithTag("Player");
            playerBody.GetComponent<GoldAmount>().AddGold(50);
            Destroy(gameObject);
        }
        
    }

    public void KnockbackFrom(Transform source, float speed, float time)
    {
        StartCoroutine(KnockbackRoutine(source, speed, time));
    }

    private IEnumerator KnockbackRoutine(Transform source, float speed, float time)
    {
        agent.isStopped = true;
        iFrame = true;

        Vector3 dir = (transform.position - source.position);
        dir.y = 0f;
        dir = dir.normalized;

        float t = 0f;
        while (t < time)
        {
            agent.Move(dir * speed * Time.deltaTime);
            t += Time.deltaTime;
            yield return null;
        }

        agent.isStopped = false;
        iFrame = false;
    }
}