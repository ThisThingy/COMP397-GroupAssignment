using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class HotbarUI : MonoBehaviour
{
    public Inventory inventory;
    public GameObject slotPrefab;
    public Transform slotParent;

    public int hotbarSize = 5;

    private InventorySlotUI[] slotUIs;
    private int selectedIndex = 0;

    void Start()
    {
        slotUIs = new InventorySlotUI[hotbarSize];

        for (int i = 0; i < hotbarSize; i++)
        {
            GameObject obj = Instantiate(slotPrefab, slotParent);
            slotUIs[i] = obj.GetComponent<InventorySlotUI>();
        }

        inventory.OnInventoryChanged += UpdateUI;

        UpdateUI();
        UpdateSelection();
    }

    void Update()
    {
        // Number keys 1�5
        for (int i = 0; i < hotbarSize; i++)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame && i == 0) SelectSlot(0);
            if (Keyboard.current.digit2Key.wasPressedThisFrame && i == 1) SelectSlot(1);
            if (Keyboard.current.digit3Key.wasPressedThisFrame && i == 2) SelectSlot(2);
            if (Keyboard.current.digit4Key.wasPressedThisFrame && i == 3) SelectSlot(3);
            if (Keyboard.current.digit5Key.wasPressedThisFrame && i == 4) SelectSlot(4);
        }
    }

    void UpdateUI()
    {
        int count = inventory.slots.Count;

        for (int i = 0; i < hotbarSize; i++)
        {
            if (i < count)
                slotUIs[i].UpdateSlot(inventory.slots[i]);
            else
                slotUIs[i].UpdateSlot(null); 
        }
    }

    void SelectSlot(int index)
    {
        selectedIndex = index;
        UpdateSelection();

        if (index < inventory.slots.Count)
            Debug.Log("Selected: " + inventory.slots[index].item?.itemName);
        else
            Debug.Log("Selected empty slot: " + index);
    }

    void UpdateSelection()
    {
        for (int i = 0; i < slotUIs.Length; i++)
        {
            Image bg = slotUIs[i].GetComponent<Image>();

            if (i == selectedIndex)
                bg.color = Color.yellow; // highlighted
            else
                bg.color = Color.white;
        }
    }
}