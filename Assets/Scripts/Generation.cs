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
    private List<Vector3> pastPositions;
    private GameObject temp;
    private Dictionary<Vector3, RoomController> roomMap;

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
        pastPositions = new List<Vector3>();
        roomMap = new Dictionary<Vector3, RoomController>();
        GameObject startRoom = Instantiate(rooms[0], currentPos, Quaternion.identity,root);//makes start room
        RoomController startRC = startRoom.GetComponent<RoomController>();
        if (startRC != null) {
            roomMap[currentPos] = startRC;
            Debug.Log($"[Generation] Start room created at {currentPos} with RoomController");
        } else {
            Debug.LogWarning($"[Generation] Start room at {currentPos} has NO RoomController component!");
        }
        pastPositions.Add(currentPos);
        direction = "posx";
        currentPos.x+=10;
        for(int i = 1; i < numRooms-2; i++){
            int rand;
            // record positions only after a room is actually instantiated (see below)
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
                GameObject roomObj = Instantiate(rooms[5], currentPos, Quaternion.Euler(0, rotation, 0),root);
                RoomController rc = roomObj.GetComponent<RoomController>();
                if (rc != null) roomMap[currentPos] = rc;
                pastPositions.Add(currentPos);
                direction = branchDir;
                currentPos = branchPos;
                rotation = branchRotation;
                //can't do this bc it overwrites an position in past positions, need to find a way to not overwrite positions in past positions when branching
            }
            

            if (rand == 1){//straight
                if(checkNextPos(currentPos, direction))
                {
                    GameObject roomObj = Instantiate(temp, currentPos, Quaternion.Euler(0, rotation, 0),root);
                    Room room = roomObj.GetComponent<Room>();
                    RoomController rc = roomObj.GetComponent<RoomController>();
                    if (rc != null) roomMap[currentPos] = rc;
                    room.player = this.player;
                    room.playerBody = this.playerBody;
                    pastPositions.Add(currentPos);
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
                    GameObject roomObj = Instantiate(temp, currentPos, Quaternion.Euler(0, rotation, 0),root);
                    RoomController rc = roomObj.GetComponent<RoomController>();
                    if (rc != null) roomMap[currentPos] = rc;
                    pastPositions.Add(currentPos);
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
                    GameObject roomObj = Instantiate(temp, currentPos, Quaternion.Euler(0, rotation, 0),root);
                    RoomController rc = roomObj.GetComponent<RoomController>();
                    if (rc != null) roomMap[currentPos] = rc;
                    pastPositions.Add(currentPos);
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
                    GameObject roomObj = Instantiate(temp, currentPos, Quaternion.Euler(0, rotation, 0),root);
                    RoomController rc = roomObj.GetComponent<RoomController>();
                    if (rc != null) roomMap[currentPos] = rc;
                    pastPositions.Add(currentPos);
                    maxNumBranches-=1;
                    branching = true;
                    branchPos = newPos(currentPos,d2);
                    branchDir = d2;
                    currentPos = newPos(currentPos, d1);
                    branchesLeft = numBranchesLeft(i);
                    branchRotation = rotation+90;
                    rotation -= 90;
                    direction = d1;
                }
                else{
                    i--;
                }
            }

            //start of random decor elements
            if((Random.Range(0f,1f) < .5f)){
                decorPositions.Add(currentPos);
            }
        }

        GameObject endRoom = Instantiate(rooms[5], currentPos, Quaternion.Euler(0, rotation, 0),root);//makes end room
        RoomController endRC = endRoom.GetComponent<RoomController>();
        if (endRC != null) roomMap[currentPos] = endRC;
        pastPositions.Add(currentPos);
        GameObject chest = decor[decor.Length-1];
        Instantiate(chest, addRandomOffset(currentPos), Quaternion.Euler(0, rotation, 0), root);
        
        surface.BuildNavMesh();//If you move this behind decor gen, the enemies will avoid the decor unless moved by player 

        foreach(Vector3 decorPos in decorPositions){
            int decorRand = Random.Range(0, decor.Length-1);
            GameObject decorTemp = decor[decorRand];
            Instantiate(decorTemp, addRandomOffset(decorPos), randomRotation(), root);
        }

        // Connect adjacent rooms
        ConnectAdjacentRooms();

        // Disable all rooms initially (culling)
        foreach (var room in roomMap.Values)
        {
            if (room != null)
            {
                room.SetRoomActive(false);
                Debug.Log($"[Generation] Initially disabling room: {room.gameObject.name}");
            }
        }
        Debug.Log($"[Generation] All rooms disabled. Waiting for player to enter first room...");
    }

    private void ConnectAdjacentRooms()
    {
        Debug.Log($"[Generation] ConnectAdjacentRooms() called with {roomMap.Count} rooms in map");
        int connectionCount = 0;

        // Check all rooms and link adjacent ones
        foreach (var pos in roomMap.Keys)
        {
            RoomController room = roomMap[pos];
            if (room == null) {
                Debug.LogWarning($"[Generation] Room at {pos} is null in roomMap!");
                continue;
            }
            if (room.connectedRooms == null) {
                Debug.LogWarning($"[Generation] Room at {pos} has null connectedRooms list!");
                continue;
            }

            // Check all 4 directions (±10 on x, ±10 on z)
            Vector3[] adjacentPositions = new Vector3[]
            {
                pos + new Vector3(10, 0, 0),  // +X
                pos - new Vector3(10, 0, 0),  // -X
                pos + new Vector3(0, 0, 10),  // +Z
                pos - new Vector3(0, 0, 10)   // -Z
            };

            foreach (var adjPos in adjacentPositions)
            {
                if (roomMap.ContainsKey(adjPos))
                {
                    RoomController adjRoom = roomMap[adjPos];
                    if (adjRoom != null && !room.connectedRooms.Contains(adjRoom))
                    {
                        room.connectedRooms.Add(adjRoom);
                        connectionCount++;
                        Debug.Log($"[Generation] Connected room at {pos} to room at {adjPos}");
                    }
                }
            }
        }
        Debug.Log($"[Generation] ConnectAdjacentRooms() complete. Total connections made: {connectionCount}");
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
        for (int i = 0; i < pastPositions.Count; i++)
        {
            if (pastPositions[i] == pos)
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
