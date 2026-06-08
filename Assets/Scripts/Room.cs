// Room spawner: optional enemy spawn on Start based on spawnChance.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Transform spawnPoint;
    public float spawnChance;
    public GameObject toSpawn;
    public Transform player;
    public GameObject playerBody;
    // Start is called before the first frame update
    void Start()
    {
        float num = Random.Range(0.0f,1.0f);
        if(num<=spawnChance){
            Quaternion spawnRotation = transform.rotation * Quaternion.Euler(0f, -90f, 0f);
            Enemy enemy = Instantiate(toSpawn, spawnPoint.position, spawnRotation).GetComponent<Enemy>();
            enemy.player = player;
            enemy.playerBody = playerBody;
        }
    }
}
