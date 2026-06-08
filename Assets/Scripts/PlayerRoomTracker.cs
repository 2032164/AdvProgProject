// Tracks the player's current room and manages nearby room visibility using
// breadth-first traversal up to renderDepth.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRoomTracker : MonoBehaviour
{
    private RoomController currentRoom;
    [Min(0)] public int renderDepth = 2;

    private void OnTriggerEnter(Collider other)
    {
        RoomController room = other.GetComponentInParent<RoomController>();
        if (room != null && room != currentRoom)
        {
            EnterRoom(room);
        }
    }

    private void EnterRoom(RoomController room)
    {
        currentRoom = room;
        RefreshRoomVisibility(currentRoom);
    }

    //Used AI to help with the logic of this method, it is a breadth first search that starts at the current room and adds connected rooms to a queue until it reaches the render depth. It then sets all rooms that are not in the queue to inactive.
    private void RefreshRoomVisibility(RoomController centerRoom)
    {
        if (centerRoom == null)
        {
            return;
        }

        HashSet<RoomController> roomsToKeepActive = new HashSet<RoomController>();
        // Breadth-first search from the player's current room to determine which
        // rooms should remain active (rendered). `roomsToKeepActive` prevents
        // revisiting rooms and limits traversal to `renderDepth`.
        Queue<(RoomController room, int depth)> roomsToVisit = new Queue<(RoomController room, int depth)>();

        roomsToVisit.Enqueue((centerRoom, 0));

        while (roomsToVisit.Count > 0)
        {
            var (room, depth) = roomsToVisit.Dequeue();
            if (room == null || roomsToKeepActive.Contains(room) || depth > renderDepth)
            {
                continue;
            }

            roomsToKeepActive.Add(room);

            if (depth < renderDepth && room.connectedRooms != null)
            {
                foreach (var connected in room.connectedRooms)
                {
                    if (connected != null)
                    {
                        roomsToVisit.Enqueue((connected, depth + 1));
                    }
                }
            }
        }


        foreach (var room in RoomController.AllRooms)
        {
            if (room == null)
            {
                continue;
            }

            bool shouldBeActive = roomsToKeepActive.Contains(room);
            room.SetRoomActive(shouldBeActive);
        }
    }
}
