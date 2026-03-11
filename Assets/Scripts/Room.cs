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
            Enemy enemy = Instantiate(toSpawn,spawnPoint.position,Quaternion.identity).GetComponent<Enemy>();
            enemy.player = player;
            enemy.playerBody = playerBody;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
