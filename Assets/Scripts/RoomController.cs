using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public static readonly List<RoomController> AllRooms = new List<RoomController>();

    // Assign connected rooms in the gen code
    public List<RoomController> connectedRooms;

    private void Awake()
    {
        if (connectedRooms == null)
        {
            connectedRooms = new List<RoomController>();
        }
        if (!AllRooms.Contains(this))
        {
            AllRooms.Add(this);
        }
    }

    private void OnDestroy()
    {
        AllRooms.Remove(this);
    }

    public void SetRoomActive(bool active)
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers)
        {
            if (r != null) {
                r.enabled = active;
            }
        }
    }
}
