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

    private void RefreshRoomVisibility(RoomController centerRoom)
    {
        if (centerRoom == null)
        {
            return;
        }

        HashSet<RoomController> roomsToKeepActive = new HashSet<RoomController>();
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
