using NUnit.Framework.Interfaces;
using UnityEngine;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Survival/Crafting Recipe")]
public class CraftingRecipe : ScriptableObject
{
    [System.Serializable]
    public struct Ingredient
    {
        public ItemData item;
        public int amount;
    }

    public Ingredient[] ingredients;

    public ItemData result;
    public int resultAmount = 1;
}