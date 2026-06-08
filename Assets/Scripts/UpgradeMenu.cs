// Upgrade menu handler: triggers hotbar rerolls from UI buttons.

using System.ComponentModel;
using UnityEngine;

public class UpgradeMenu : MonoBehaviour
{
    [SerializeField] private HotBar hotBar;
    [SerializeField] private Canvas canvas;
    private FPSController fpsController;

    private void Start()
    {
        if (fpsController == null)
        {
            fpsController = FindAnyObjectByType<FPSController>();
        }
    }

    private void OnEnable()
    {
        UnityEngine.Debug.Log("Upgrade menu enabled, unlocking cursor and preventing player movement");
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }


    private void EnsureHotBar()
    {
        if (hotBar == null)
        {
            hotBar = FindAnyObjectByType<HotBar>();
        }
    }

    private void UpgradeSlot(int slotIndex)
    {
        EnsureHotBar();
        if (hotBar == null)
        {
            return;
        }

        hotBar.RerollSlot(slotIndex);
    }

    private void CloseMenu()
    {
        UnityEngine.Debug.Log("Closing upgrade menu, locking cursor and allowing player movement");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (fpsController != null)
        {
            fpsController.canMove = true;
        }
        canvas.enabled = false;
    }

    public void buttonOne()
    {
        UpgradeSlot(0);
        UnityEngine.Debug.Log("Button one clicked, rerolling slot 0");
        CloseMenu();
    }

    public void buttonTwo()
    {
        UpgradeSlot(1);
        UnityEngine.Debug.Log("Button two clicked, rerolling slot 1");
        CloseMenu();
    }

    public void buttonThree()
    {
        UnityEngine.Debug.Log("Button three clicked, exiting upgrade menu");
        CloseMenu();
    }
}
