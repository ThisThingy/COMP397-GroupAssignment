using UnityEngine;
using UnityEngine.UI;

public class CraftingUI : MonoBehaviour
{
    public CraftingSystem craftingSystem;
    public CraftingRecipe[] recipes;

    public GameObject craftButtonPrefab;
    public Transform buttonParent;

    void Start()
    {
        foreach (CraftingRecipe recipe in recipes)
        {
            GameObject obj = Instantiate(craftButtonPrefab, buttonParent);
            Button button = obj.GetComponent<Button>();

            Text buttonText = obj.GetComponentInChildren<Text>();
            buttonText.text = "Craft " + recipe.result.itemName;
            button.onClick.AddListener(() => TryCraft(recipe));
        }
    }

    void TryCraft(CraftingRecipe recipe)
    {
        if (craftingSystem.CanCraft(recipe))
        {
            craftingSystem.Craft(recipe);
        }
        else
        {
            Debug.Log("Not enough materials.");
        }
    }
}