using UnityEngine;

public class CraftingSystem : MonoBehaviour
{
    public Inventory playerInventory;

    public bool CanCraft(CraftingRecipe recipe)
    {
        foreach (var ingredient in recipe.ingredients)
        {
            if (!playerInventory.HasItem(ingredient.item, ingredient.amount))
                return false;
        }

        return true;
    }

    public void Craft(CraftingRecipe recipe)
    {
        if (!CanCraft(recipe))
        {
            Debug.Log("Not enough materials!");
            return;
        }

        // Remove ingredients
        foreach (var ingredient in recipe.ingredients)
        {
            playerInventory.RemoveItem(ingredient.item, ingredient.amount);
        }

        // Add result
        playerInventory.AddItem(recipe.result, recipe.resultAmount);

        Debug.Log("Crafted: " + recipe.result.itemName);
    }
}