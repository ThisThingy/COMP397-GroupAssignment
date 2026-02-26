using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryToggle : MonoBehaviour
{
    private Canvas canvas;

    void Awake()
    {
        canvas = GetComponent<Canvas>();
        canvas.enabled = false; // Start hidden
    }

    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            canvas.enabled = !canvas.enabled;
        }
    }
}