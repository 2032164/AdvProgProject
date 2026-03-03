using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using Debug = UnityEngine.Debug;

public class Generation : MonoBehaviour
{
    // Start is called before the first frame update
    //offset is 10?
    public GameObject[] rooms;//1= start, 2= newPos, 3= left turn, 4 = right turn
    public int numRooms = 10;
    private Vector3 currentPos;
    private int rotation;
    private string direction;
    private Vector3[] pastPositions;
    


    //every left turn = -90 deg of rotation and switches to moving on z axis
    //every right turn = +90 deg of rotation and switches to moving on -z axis

 
    void Start()
    {
        pastPositions = new Vector3[numRooms];
        Instantiate(rooms[0], currentPos, Quaternion.identity);//makes start room
        pastPositions[0] = currentPos;
        direction = "posx";
        currentPos = new Vector3(10, 0, 0);
        for(int i = 1; i < numRooms; i++)
        {
            int rand = Random.Range(1, rooms.Length);//randomly selects newPos, left turn, or right turn
            GameObject temp = rooms[rand];//selects random room

            if (rand == 1)//straight
            {
                if(checkNextPos(currentPos, direction))
                {
                    //Debug.Log("newPos" + currentPos + direction);
                    Instantiate(temp, currentPos, Quaternion.Euler(0, rotation, 0));
                    currentPos = newPos(currentPos, direction);
                }
                else{
                    Debug.Log("trying straight - position already occupied" + currentPos + direction);
                    i--;
                }
            }
            else if (rand == 2 )//left turn
            {

                string tempDirection = direction switch{
                    "posx" => "posz",
                    "negx" => "negz",
                    "posz" => "negx",
                    "negz" => "posx",
                    _ => direction
                };

                if(checkNextPos(currentPos, tempDirection))
                {
                    //Debug.Log("left turn" + currentPos + direction);
                    Instantiate(temp, currentPos, Quaternion.Euler(0, rotation, 0));
                    rotation -= 90;
                    direction = tempDirection;
                    currentPos = newPos(currentPos, direction);
                }
                else{
                    Debug.Log("trying left turn - position already occupied" + currentPos + direction);
                    i--;
                }
            }
            else if (rand == 3)//right turn
            {
                string tempDirection = direction switch{
                    "posx" => "negz",
                    "negx" => "posz",
                    "posz" => "posx",
                    "negz" => "negx",
                    _ => direction
                };
                if(checkNextPos(currentPos, tempDirection))
                {
                //Debug.Log("right turn" + currentPos + direction);
                Instantiate(temp, currentPos, Quaternion.Euler(0, rotation, 0));
                rotation += 90;
                direction = tempDirection;
                currentPos = newPos(currentPos, direction);
                }
                else{
                    Debug.Log("trying right turn - position already occupied" + currentPos + direction);
                    i--; 
                }
            }
            pastPositions[i] = currentPos;
        }
    }

     private Vector3 newPos(Vector3 currentPos, string direction)
     //finds the next position based on the current position and direction, adds 10 to the appropriate axis
    {
        if (direction == "posx")
        {
            return new Vector3(currentPos.x + 10, currentPos.y, currentPos.z);
        }
        else if (direction == "negx")
        {
            return new Vector3(currentPos.x - 10, currentPos.y, currentPos.z);
        }
        else if (direction == "posz")
        {
            return new Vector3(currentPos.x, currentPos.y, currentPos.z + 10);
        }
        else//negz
        {
            return new Vector3(currentPos.x, currentPos.y, currentPos.z - 10);
        }
    }

    private bool checkNextPos(Vector3 pos, string direction)
    {
        pos = newPos(pos, direction);
        //finds the next position and checks if it has a room already, if it does then it returns false
        for (int i = 0; i < pastPositions.Length; i++)
        {
            if (pastPositions[i] == pos)
            {
                return false;
            }
        }
        return true;
    }
}
