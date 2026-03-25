using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using UnityEngine;
using Unity.AI.Navigation;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using Debug = UnityEngine.Debug;
using System.Runtime.InteropServices;

public class Generation : MonoBehaviour
{
    // Start is called before the first frame update
    //offset is 10?
    public GameObject[] rooms;//1= start, 2= straight, 3= left turn, 4 = right turn, 5 = branch, 6 = end
    public GameObject[] decor;
    public int numRooms = 10;
    public int maxNumBranches = 1;
    public Transform root;
    public NavMeshSurface surface;
    public Transform player;
    public GameObject playerBody;

    private Vector3 currentPos;
    private int rotation;
    private string direction;
    private Vector3[] pastPositions;
    private GameObject temp;

    //every left turn = -90 deg of rotation and switches to moving on z axis
    //every right turn = +90 deg of rotation and switches to moving on -z axis



///FOR SOME REASON, Three right turn keep happening and they cause it to double back on its self
 
    void Start(){
        ArrayList decorPositions = new ArrayList();
        bool branching = false;
        Vector3 branchPos = new Vector3(0,0,0);
        string branchDir = null;;
        int branchesLeft = 0;
        int branchRotation = 0;
        GameObject temp = null;
        pastPositions = new Vector3[numRooms+1];
        Instantiate(rooms[0], currentPos, Quaternion.identity,root);//makes start room
        pastPositions[0] = currentPos;
        direction = "posx";
        currentPos.x+=10;
        ///FOR SOMEREASON IF ITS A STRAIGHT HALLWAY AT THE START,IT NEVER GETS ADDED TO PAST POSITIONS
        for(int i = 1; i < numRooms-2; i++){
            int rand;
            pastPositions[i] = currentPos;//this has to be here bc or else it only records the position after the room is made aka the next position sothin in he dont work
            if(maxNumBranches > 0 && !branching){
                rand = Random.Range(1, rooms.Length-1);
            }
            else{
                rand = Random.Range(1, rooms.Length-2);//randomly selects newPos, left turn, or right turn
            }
            temp = rooms[rand];
            if (branching)
            {
                branchesLeft--;
                //Debug.Log("branching, branches left: " + branchesLeft + " current pos: " + currentPos + "direction: " + direction);
            }
            if(branching && branchesLeft == 0)
            {
                //Debug.Log("Done with branch, returning to main path. Current pos: " + currentPos + " branch pos: " + branchPos + " branch dir: " + branchDir + " direction: " + direction);
                branching = false;
                Instantiate(rooms[5], currentPos, Quaternion.Euler(0, rotation, 0),root);
                direction = branchDir;
                currentPos = branchPos;
                rotation = branchRotation;
                //can't do this bc it overwrites an position in past positions, need to find a way to not overwrite positions in past positions when branching
            }
            

            if (rand == 1){//straight
                if(checkNextPos(currentPos, direction))
                {
                    Room room = Instantiate(temp, currentPos, Quaternion.Euler(0, rotation, 0),root).GetComponent<Room>();
                    room.player = this.player;
                    room.playerBody = this.playerBody;
                    currentPos = newPos(currentPos, direction);
                }
                else{
                    i--;
                }
            }
            else if (rand == 2 ){//left turn
            
                string tempDirection = leftTurn(direction);

                if(checkNextPos(currentPos, tempDirection)){
                    //Debug.Log("left turn" + currentPos + direction);
                    Instantiate(temp, currentPos, Quaternion.Euler(0, rotation, 0),root);
                    rotation -= 90;
                    direction = tempDirection;
                    currentPos = newPos(currentPos, direction);
                }
                else{
                    //Debug.Log("trying left turn - position already occupied" + currentPos + direction);
                    i--;
                }
            }
            else if (rand == 3){//right turn

                string tempDirection = rightTurn(direction);
                if(checkNextPos(currentPos, tempDirection)){
                //Debug.Log("right turn" + currentPos + direction);
                    Instantiate(temp, currentPos, Quaternion.Euler(0, rotation, 0),root);
                    rotation += 90;
                    direction = tempDirection;
                    currentPos = newPos(currentPos, direction);
                }
                else{
                    //Debug.Log("trying right turn - position already occupied" + currentPos + direction);
                    i--; 
                }
            }
            else if (rand == 4 && i<numRooms-3){//branch
                string d1 = leftTurn(direction);
                string d2 = rightTurn(direction);
                if(checkNextPos(currentPos, d1) && checkNextPos(currentPos, d2)){//not cjhecking right
                    //Debug.Log("branch" + currentPos + direction);
                    Instantiate(temp, currentPos, Quaternion.Euler(0, rotation, 0),root);
                    maxNumBranches-=1;
                    branching = true;
                    branchPos = newPos(currentPos,d2);
                    branchDir = d2;
                    currentPos = newPos(currentPos, d1);
                    branchesLeft = numBranchesLeft(i);
                    branchRotation = rotation+90;
                    rotation -= 90;
                    direction = d1;
                    //Debug.Log("trying branch" + rotation + currentPos + direction + " branch pos: " + branchPos + " branch dir: " + branchDir + " branches rotation: " + branchRotation + " branches left: " + branchesLeft + "max branches: " + maxNumBranches);
                }
                else{
                    i--;
                }
            }

            //start of random decor elements
            if(Random.Range(0f,1f) < .5f){
                decorPositions.Add(currentPos);
            }
        }
        Instantiate(rooms[5], currentPos, Quaternion.Euler(0, rotation, 0),root);//makes end room
        surface.BuildNavMesh();
        foreach(Vector3 decorPos in decorPositions){
            int decorRand = Random.Range(0, decor.Length);
            GameObject decorTemp = decor[decorRand];
            Instantiate(decorTemp, addRandomOffset(decorPos), randomRotation(), root);
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
            if (pastPositions[i] == pos)//idk if this works
            {
                return false;
            }
        }
        return true;
    }

    private string rightTurn(string direction)
    {
        return direction switch
        {
            "posx" => "negz",
            "negx" => "posz",
            "posz" => "posx",
            "negz" => "negx",
            _ => direction
        };
    }

    private string leftTurn(string direction)
    {
        return direction switch
        {
            "posx" => "posz",
            "negx" => "negz",
            "posz" => "negx",
            "negz" => "posx",
            _ => direction
        };
    }

    private int numBranchesLeft(int i)//SMTING ABOOUT WHEN I MINUS WHEN WHEN IT CAN"T GENERATE??????????
    {
        if((numRooms - i)%2 == 0)
        {
            return (numRooms - i) / 2;
        }
        else
        {
            return ((numRooms - i) / 2) - 1;
        }
    }

    private Vector3 addRandomOffset(Vector3 pos)
    {
        float xOffset = randomOutsideCenter(-3f, 3f, -1f, 1f);
        float zOffset = randomOutsideCenter(-3f, 3f, -1f, 1f);
        
        return new Vector3(pos.x + xOffset, pos.y, pos.z + zOffset);
    }

    private float randomOutsideCenter(float min, float max, float blockedMin, float blockedMax)
    {
        float leftSize = blockedMin - min;
        float rightSize = max - blockedMax;
        float totalSize = leftSize + rightSize;

        float pick = Random.Range(0f, totalSize);
        if (pick < leftSize)
        {
            return Random.Range(min, blockedMin);
        }

        return Random.Range(blockedMax, max);
    }

    private Quaternion randomRotation()
    {
        float yRotation = Random.Range(0f, 360f);
        return Quaternion.Euler(0f, yRotation, 0f);
    }
}
