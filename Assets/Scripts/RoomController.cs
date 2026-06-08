// Groups room renderers and tracks connectivity for culling and visibility.

using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public static readonly List<RoomController> AllRooms = new List<RoomController>();
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

    // Enable or disable all renderers in this room. Used by the culling system
    // to turn rooms on or off based on player's current room pos
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
