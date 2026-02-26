using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public int inventorySize = 20;
    public List<InventorySlot> slots = new List<InventorySlot>();
    public System.Action OnInventoryChanged;
    private void Awake()
    {
        for (int i = 0; i < inventorySize; i++)
        {
            slots.Add(new InventorySlot());
        }
    }

    public bool AddItem(ItemData item, int amount)
    {
        if (item == null) return false;

        int remainingAmount = amount;
        bool changed = false;

        // Fill existing stacks
        if (item.isStackable)
        {
            foreach (InventorySlot slot in slots)
            {
                if (slot.item == item && slot.amount < item.maxStack)
                {
                    int spaceLeft = item.maxStack - slot.amount;
                    int amountToAdd = Mathf.Min(spaceLeft, remainingAmount);

                    slot.amount += amountToAdd;
                    remainingAmount -= amountToAdd;

                    changed = true;

                    if (remainingAmount <= 0)
                    {
                        OnInventoryChanged?.Invoke();
                        return true;
                    }
                }
            }
        }

        // Use empty slots
        foreach (InventorySlot slot in slots)
        {
            if (slot.IsEmpty())
            {
                int amountToAdd = item.isStackable
                    ? Mathf.Min(item.maxStack, remainingAmount)
                    : 1;

                slot.item = item;
                slot.amount = amountToAdd;

                remainingAmount -= amountToAdd;

                if (!item.isStackable)
                    remainingAmount--;

                changed = true;

                if (remainingAmount <= 0)
                {
                    OnInventoryChanged?.Invoke();
                    return true;
                }
            }
        }

        if (changed)
            OnInventoryChanged?.Invoke();

        Debug.Log("Inventory Full! Could not add all items.");
        return false;
    }

    public bool HasItem(ItemData item, int amount)
    {
        int total = 0;

        foreach (InventorySlot slot in slots)
        {
            if (slot.item == item)
                total += slot.amount;
        }

        return total >= amount;
    }

    public void RemoveItem(ItemData item, int amount)
    {
        bool changed = false;

        foreach (InventorySlot slot in slots)
        {
            if (slot.item == item)
            {
                int removeAmount = Mathf.Min(slot.amount, amount);
                slot.amount -= removeAmount;
                amount -= removeAmount;

                if (slot.amount <= 0)
                    slot.Clear();

                changed = true;

                if (amount <= 0)
                {
                    OnInventoryChanged?.Invoke();
                    return;
                }
            }
        }

        if (changed)
            OnInventoryChanged?.Invoke();
    }
}