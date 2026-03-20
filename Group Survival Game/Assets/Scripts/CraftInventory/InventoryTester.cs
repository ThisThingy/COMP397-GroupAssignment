using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryTester : MonoBehaviour
{
    public Inventory inventory;
    public CraftingSystem craftingSystem;

    public ItemData wood;
    public ItemData stone;
    public ItemData iron;
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

        // Press U to add more wood
        if (Keyboard.current.uKey.wasPressedThisFrame)
        {
            inventory.AddItem(wood, 45);
            Debug.Log("Added 45 Wood");
            PrintInventory();
        }

        // Press I to add stone
        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            inventory.AddItem(stone, 45);
            Debug.Log("Added 45 Stone");
            PrintInventory();
        }
        // Press O to add stone
        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            inventory.AddItem(iron, 45);
            Debug.Log("Added 45 Iron");
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