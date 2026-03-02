using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generation : MonoBehaviour
{
    // Start is called before the first frame update
    //offset is 8.27f
    public GameObject[] rooms;
    void Start()
    {
        for(int i = 0; i < rooms.Length; i++)
        {
            Instantiate(rooms[i], new Vector3(8.27f * i, 0, 0), Quaternion.identity);
        }
    }
}
