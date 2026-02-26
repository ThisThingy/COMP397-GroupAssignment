using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    public Image itemIcon;
    public Text stackText;

    public void UpdateSlot(InventorySlot slot)
    {
        if (slot == null || slot.IsEmpty())
        {
            itemIcon.enabled = false;
            stackText.text = "";
        }
        else
        {
            itemIcon.enabled = true;
            itemIcon.sprite = slot.item.icon;

            if (slot.item.isStackable && slot.amount > 1)
                stackText.text = slot.amount.ToString();
            else
                stackText.text = "";
        }
    }
}