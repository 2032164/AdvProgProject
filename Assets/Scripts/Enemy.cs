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
}
