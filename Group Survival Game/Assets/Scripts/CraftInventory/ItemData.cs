using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Survival/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;

    public bool isStackable = true;
    public int maxStack = 50;
}