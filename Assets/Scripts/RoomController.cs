using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public static readonly List<RoomController> AllRooms = new List<RoomController>();

    // Assign connected rooms (adjacent rooms) in prefabs or at runtime
    public List<RoomController> connectedRooms;
    private Renderer[] renderers;

    private void Awake()
    {
        if (connectedRooms == null)
        {
            connectedRooms = new List<RoomController>();
        }
        renderers = GetComponentsInChildren<Renderer>(true); // include inactive
        if (!AllRooms.Contains(this))
        {
            AllRooms.Add(this);
        }
    }

    private void OnDestroy()
    {
        AllRooms.Remove(this);
    }

    // Enable/disable rendering for this room
    public void SetRoomActive(bool active)
    {
        if (renderers == null) {
            return;
        }
        foreach (var r in renderers)
        {
            if (r != null) {
                r.enabled = active;
            }
        }
    }
}
