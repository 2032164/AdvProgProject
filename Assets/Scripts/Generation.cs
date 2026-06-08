
// Procedural dungeon/room generator. Makes a series of rooms based on prefabs
// and manages branching, room connectivity, and decor placement.

using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Unity.AI.Navigation;
using Vector3 = UnityEngine.Vector3;
using Quaternion = UnityEngine.Quaternion;
using System.Runtime.InteropServices;

public class Generation : MonoBehaviour
{
    public GameObject[] rooms;//1= start, 2= straight, 3= left turn, 4 = right turn, 5 = branch, 6 = end
    public GameObject[] decor;
    public int campfireDecorIndex = 3;
    public int campfireSpawnLimit = 1;
    public int numRooms = 10;
    public int maxNumBranches = 1;
    public Transform root;
    public NavMeshSurface surface;
    public Transform player;
    public GameObject playerBody;

    private Vector3 currentPos;
    private Vector3 lastPlacedPos;
    private Vector3 branchJunctionPos;
    private bool waitingForBranchSecondStart;
    private int campfireSpawnCount;
    private int rotation;
    private string direction;
    private List<Vector3> pastPositions;
    private GameObject temp;
    private Dictionary<Vector3, RoomController> roomMap;

    //every left turn = -90 deg of rotation and switches to moving on z axis
    //every right turn = +90 deg of rotation and switches to moving on -z axis



    //This constructs rooms sequentially and handles
    // branching. The generator records placed positions in pastPositions for checking placement and
    // uses roomMap to track instantiated RoomController objects for connectivity
    // The main loop attempts to place numRooms rooms, when it detects a dead-end, where no rooms can be placed
    // it breaks early and places the end room to avoid infinite retries when stuck
    void Start(){
        ArrayList decorPositions = new ArrayList();
        bool branching = false;
        Vector3 branchPos = new Vector3(0,0,0);
        string branchDir = null;;
        int branchesLeft = 0;
        int branchRotation = 0;
        GameObject temp = null;
        campfireSpawnCount = 0;
        pastPositions = new List<Vector3>();
        roomMap = new Dictionary<Vector3, RoomController>();
        GameObject startRoom = Instantiate(rooms[0], currentPos, Quaternion.identity,root);//makes start room
        RoomController startRC = startRoom.GetComponent<RoomController>();
        if (startRC != null) {
            roomMap[currentPos] = startRC;
        }
        lastPlacedPos = currentPos;
        pastPositions.Add(currentPos);
        direction = "posx";
        currentPos.x+=10;
        for(int i = 1; i < numRooms-2; i++){
            if (!HasAnyValidPlacement(direction, branching, i))
            {
                Debug.LogWarning("Generation reached a dead-end. Placing end room early.");
                break;
            }

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
            }
            if(branching && branchesLeft == 0)
            {
                //Debug.Log("Done with branch, returning to main path. Current pos: " + currentPos + " branch pos: " + branchPos + " branch dir: " + branchDir + " direction: " + direction);
                branching = false;
                GameObject roomObj = Instantiate(rooms[5], currentPos, Quaternion.Euler(0, rotation, 0),root);
                RoomController rc = roomObj.GetComponent<RoomController>();
                if (rc != null)
                {
                    roomMap[currentPos] = rc;
                    ConnectRooms(lastPlacedPos, currentPos);
                }
                pastPositions.Add(currentPos);
                lastPlacedPos = currentPos;
                direction = branchDir;
                currentPos = branchPos;
                rotation = branchRotation;
                waitingForBranchSecondStart = true;
                // After finishing a branch, return to the previously saved main location.
            }
            

            if (rand == 1){//straight hall
                if(checkNextPos(currentPos, direction))
                {
                    GameObject roomObj = Instantiate(temp, currentPos, Quaternion.Euler(0, rotation, 0),root);
                    Room room = roomObj.GetComponent<Room>();
                    RoomController rc = roomObj.GetComponent<RoomController>();
                    if (rc != null)
                    {
                        roomMap[currentPos] = rc;
                        ConnectRooms(lastPlacedPos, currentPos);
                        if (waitingForBranchSecondStart)
                        {
                            ConnectRooms(branchJunctionPos, currentPos);
                            waitingForBranchSecondStart = false;
                        }
                    }
                    room.player = this.player;
                    room.playerBody = this.playerBody;
                    pastPositions.Add(currentPos);
                    lastPlacedPos = currentPos;
                    currentPos = newPos(currentPos, direction);
                }
                else{
                    // Placement failed because the target position is occupied or would be blocked off.
                    // Reset the loop counter to retry this step with a different room choice.
                    i--;
                }
            }
            else if (rand == 2 ){//left turn
            
                string tempDirection = leftTurn(direction);

                if(checkNextPos(currentPos, tempDirection)){
                    //Debug.Log("left turn" + currentPos + direction);
                    GameObject roomObj = Instantiate(temp, currentPos, Quaternion.Euler(0, rotation, 0),root);
                    RoomController rc = roomObj.GetComponent<RoomController>();
                    if (rc != null)
                    {
                        roomMap[currentPos] = rc;
                        ConnectRooms(lastPlacedPos, currentPos);
                        if (waitingForBranchSecondStart)
                        {
                            ConnectRooms(branchJunctionPos, currentPos);
                            waitingForBranchSecondStart = false;
                        }
                    }
                    pastPositions.Add(currentPos);
                    lastPlacedPos = currentPos;
                    rotation -= 90;
                    direction = tempDirection;
                    currentPos = newPos(currentPos, direction);
                }
                else{
                    // Left-turn would collide, retries
                    i--;
                }
            }
            else if (rand == 3){//right turn

                string tempDirection = rightTurn(direction);
                if(checkNextPos(currentPos, tempDirection)){
                //Debug.Log("right turn" + currentPos + direction);
                    GameObject roomObj = Instantiate(temp, currentPos, Quaternion.Euler(0, rotation, 0),root);
                    RoomController rc = roomObj.GetComponent<RoomController>();
                    if (rc != null)
                    {
                        roomMap[currentPos] = rc;
                        ConnectRooms(lastPlacedPos, currentPos);
                        if (waitingForBranchSecondStart)
                        {
                            ConnectRooms(branchJunctionPos, currentPos);
                            waitingForBranchSecondStart = false;
                        }
                    }
                    pastPositions.Add(currentPos);
                    lastPlacedPos = currentPos;
                    rotation += 90;
                    direction = tempDirection;
                    currentPos = newPos(currentPos, direction);
                }
                else{
                    // Right-turn would collide, retries
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
                    if (rc != null)
                    {
                        roomMap[currentPos] = rc;
                        ConnectRooms(lastPlacedPos, currentPos);
                    }
                    pastPositions.Add(currentPos);
                    lastPlacedPos = currentPos;
                    branchJunctionPos = currentPos;
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
                    // Branch placement was invalid, retries
                    i--;
                }
            }

            //start of random decor elements placement, 50% chance
            if((Random.Range(0f,1f) < .5f)){
                decorPositions.Add(currentPos);
            }
        }

        GameObject endRoom = Instantiate(rooms[5], currentPos, Quaternion.Euler(0, rotation, 0),root);//Makes end room at end of path
        RoomController endRC = endRoom.GetComponent<RoomController>();
        if (endRC != null)
        {
            roomMap[currentPos] = endRC;
            ConnectRooms(lastPlacedPos, currentPos);
            // The final end room should terminate the path
            waitingForBranchSecondStart = false;
        }
        pastPositions.Add(currentPos);

        surface.BuildNavMesh();//If you move this behind decor gen, the enemies will avoid the decor unless moved by player 

        //generates decor elements at the positions recorded in decorPositions, with random rotation and offset for variety.
        //uses GetRandomDecorPrefab to select decor, which handles campfire placement limits.
        foreach(Vector3 decorPos in decorPositions){
            GameObject decorTemp = GetRandomDecorPrefab();
            Transform roomParent = GetRoomTransformAtPosition(decorPos);
            Instantiate(decorTemp, addRandomOffset(decorPos), randomRotation(),roomParent);
        }

        // Disable all rooms initially to help preformace, they are enabled as the player generates
        foreach (var room in roomMap.Values)
        {
            if (room != null)
            {
                room.SetRoomActive(false);
            }
        }
    }

    //connects two rooms in the roomMap by adding each other to their connectedRooms list.
    //For use in culling extra objects
    private void ConnectRooms(Vector3 fromPos, Vector3 toPos)
    {
        if (!roomMap.TryGetValue(fromPos, out RoomController fromRoom) || fromRoom == null)
        {
            return;
        }

        if (!roomMap.TryGetValue(toPos, out RoomController toRoom) || toRoom == null)
        {
            return;
        }

        if (!fromRoom.connectedRooms.Contains(toRoom))
        {
            fromRoom.connectedRooms.Add(toRoom);
        }

        if (!toRoom.connectedRooms.Contains(fromRoom))
        {
            toRoom.connectedRooms.Add(fromRoom);
        }
    }

    //gets random decor prefab, with logic to limit campfire spawns
    private GameObject GetRandomDecorPrefab()
    {
        if (decor == null || decor.Length == 0)
        {
            return null;
        }

        int safety = 0;
        while (safety < 20)
        {
            safety++;
            int decorRand = Random.Range(0, decor.Length - 1);
            GameObject decorTemp = decor[decorRand];

            if (campfireDecorIndex >= 0 && campfireDecorIndex < decor.Length && decorTemp == decor[campfireDecorIndex])
            {
                if (campfireSpawnCount >= campfireSpawnLimit)
                {
                    continue;
                }

                campfireSpawnCount++;
            }

            return decorTemp;
        }

        return decor[0];
    }

     private Vector3 newPos(Vector3 currentPos, string direction)
     //finds the next position based on the current position and direction, adds 10 to the appropriate axis so pos is updates
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
    //finds the next position and checks if it has a room already, if it does then it returns false
    private bool checkNextPos(Vector3 pos, string direction)
    {
        pos = newPos(pos, direction);
        
        for (int i = 0; i < pastPositions.Count; i++)
        {
            if (pastPositions[i] == pos)
            {
                return false;
            }
        }
        return true;
    }

    //checks if a room can be placed at all, if one can't, then the generator is stuck and needs to end
    private bool HasAnyValidPlacement(string currentDirection, bool branching, int roomIndex)
    {
        if (checkNextPos(currentPos, currentDirection))
        {
            return true;
        }

        string leftDir = leftTurn(currentDirection);
        if (checkNextPos(currentPos, leftDir))
        {
            return true;
        }

        string rightDir = rightTurn(currentDirection);
        if (checkNextPos(currentPos, rightDir))
        {
            return true;
        }

        bool canStartBranch = !branching && maxNumBranches > 0 && roomIndex < numRooms - 3;
        if (canStartBranch && checkNextPos(currentPos, leftDir) && checkNextPos(currentPos, rightDir))
        {
            return true;
        }

        return false;
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

    private int numBranchesLeft(int i)
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
    //adds a random offset to a position for decor placement 
    private Vector3 addRandomOffset(Vector3 pos)
    {
        float xOffset = randomOutsideCenter(-3f, 3f, -1f, 1f);
        float zOffset = randomOutsideCenter(-3f, 3f, -1f, 1f);
        
        return new Vector3(pos.x + xOffset, pos.y, pos.z + zOffset);
    }

// Utility method to get the RoomController at a given position from the roomMap, returns null if not found.
    private Transform GetRoomTransformAtPosition(Vector3 pos)
    {
        if (roomMap.TryGetValue(pos, out RoomController roomController) && roomController != null)
        {
            return roomController.transform;
        }

        return null;
    }

    //random generator between min and max excluding blockedMin-BlockedMax for decor placing
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
