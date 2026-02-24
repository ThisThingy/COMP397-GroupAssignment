using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryTester : MonoBehaviour
{
    public Inventory inventory;
    public CraftingSystem craftingSystem;

    public ItemData wood;
    public ItemData stone;
    public CraftingRecipe axeRecipe;

    private void Start()
    {
        Debug.Log("=== TEST START ===");

        inventory.AddItem(wood, 5);
        inventory.AddItem(stone, 3);

        PrintInventory();
    }

    private void Update()
    {
        // Press C to craft
        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            Debug.Log("Trying to craft Axe...");

            if (craftingSystem.CanCraft(axeRecipe))
            {
                craftingSystem.Craft(axeRecipe);
                PrintInventory();
            }
            else
            {
                Debug.Log("Not enough materials.");
            }
        }

        // Press A to add more wood
        if (Keyboard.current.aKey.wasPressedThisFrame)
        {
            inventory.AddItem(wood, 45);
            Debug.Log("Added 45 Wood");
            PrintInventory();
        }

        // Press S to add stone
        if (Keyboard.current.sKey.wasPressedThisFrame)
        {
            inventory.AddItem(stone, 45);
            Debug.Log("Added 45 Stone");
            PrintInventory();
        }
    }

    void PrintInventory()
    {
        Debug.Log("---- Inventory ----");

        foreach (var slot in inventory.slots)
        {
            if (!slot.IsEmpty())
            {
                Debug.Log(slot.item.itemName + " x" + slot.amount);
            }
        }
    }
}