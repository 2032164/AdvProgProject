using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generation : MonoBehaviour
{
    // Start is called before the first frame update
    //offset is 10?
    public GameObject[] rooms;//1= start, 2= straight, 3= left turn, 4 = right turn
    public int numRooms 10f;
    private GameObject[][] map;
    private int rotation;


    //every left turn = +90 deg of rotation and z+
    //every right turn = -90 deg of rotation and z-

 
    void Start()
    {
        Instantiate(rooms[1], new Vector3(0, 0, 0), Quaternion.identity);
        for(int i = 1; i < numRooms; i++)
        {
            int rand = Random.range(1,rooms.length)
            GameObject temp = rooms[rand]

            //Instantiate(rooms[i], new Vector3(10f * i, 0, 0), Quaternion.identity);
        }
    }
}
