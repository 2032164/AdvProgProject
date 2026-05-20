using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRoomTracker : MonoBehaviour
{
    private RoomController currentRoom;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[PlayerRoomTracker] OnTriggerEnter triggered by {other.name}");
        RoomController room = other.GetComponentInParent<RoomController>();
        if (room != null && room != currentRoom)
        {
            Debug.Log($"[PlayerRoomTracker] Found RoomController, entering new room");
            EnterRoom(room);
        }
        else if (room == null)
        {
            Debug.LogWarning($"[PlayerRoomTracker] OnTriggerEnter but NO RoomController found on {other.name} or its parents!");
        }
    }

    private void EnterRoom(RoomController room)
    {
        Debug.Log($"[PlayerRoomTracker] EnterRoom called. Current room: {currentRoom?.gameObject.name ?? "null"}. New room: {room.gameObject.name}");
        currentRoom = room;
        RefreshRoomVisibility(currentRoom);
    }

    private void RefreshRoomVisibility(RoomController centerRoom)
    {
        if (centerRoom == null)
        {
            Debug.LogWarning("[PlayerRoomTracker] RefreshRoomVisibility called with null room");
            return;
        }

        HashSet<RoomController> roomsToKeepActive = new HashSet<RoomController>();
        Queue<(RoomController room, int depth)> roomsToVisit = new Queue<(RoomController room, int depth)>();

        roomsToVisit.Enqueue((centerRoom, 0));

        while (roomsToVisit.Count > 0)
        {
            var (room, depth) = roomsToVisit.Dequeue();
            if (room == null || roomsToKeepActive.Contains(room) || depth > 2)
            {
                continue;
            }

            roomsToKeepActive.Add(room);
            Debug.Log($"[PlayerRoomTracker] Keeping active room at depth {depth}: {room.gameObject.name}");

            if (depth < 2 && room.connectedRooms != null)
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

        Debug.Log($"[PlayerRoomTracker] Rooms to keep active: {roomsToKeepActive.Count}");

        foreach (var room in RoomController.AllRooms)
        {
            if (room == null)
            {
                continue;
            }

            bool shouldBeActive = roomsToKeepActive.Contains(room);
            Debug.Log($"[PlayerRoomTracker] Setting {room.gameObject.name} active={shouldBeActive}");
            room.SetRoomActive(shouldBeActive);
        }
    }
}
