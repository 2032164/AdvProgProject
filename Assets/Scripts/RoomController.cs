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
        renderers = GetComponentsInChildren<Renderer>(true); // include inactive
        Debug.Log($"[RoomController] Awake on {gameObject.name}: Found {renderers.Length} renderers");
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
        Debug.Log($"[RoomController] SetRoomActive({active}) on {gameObject.name} - {renderers?.Length ?? 0} renderers");
        if (renderers == null) {
            Debug.LogWarning($"[RoomController] renderers is null on {gameObject.name}!");
            return;
        }
        int disabledCount = 0;
        foreach (var r in renderers)
        {
            if (r != null) {
                r.enabled = active;
                if (!active) disabledCount++;
            }
        }
        if (!active) {
            Debug.Log($"[RoomController] DISABLED {disabledCount} renderers on {gameObject.name}");
        }
    }
}
