using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryToggle : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject craftingPanel;
    public PauseToggleOnUI pauseScript;

    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("E pressed");

            bool isActive = inventoryPanel.activeSelf;

            inventoryPanel.SetActive(!isActive);
            craftingPanel.SetActive(!isActive);

            pauseScript.SetInventoryOpen(!isActive);
        }
    }
}