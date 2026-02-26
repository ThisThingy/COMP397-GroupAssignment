using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public Inventory inventory;
    public GameObject slotPrefab;
    public Transform slotParent;

    private InventorySlotUI[] slotUIs;

    void Start()
    {
        slotUIs = new InventorySlotUI[inventory.slots.Count];

        for (int i = 0; i < inventory.slots.Count; i++)
        {
            GameObject obj = Instantiate(slotPrefab, slotParent);
            slotUIs[i] = obj.GetComponent<InventorySlotUI>();
        }

        inventory.OnInventoryChanged += UpdateUI;

        UpdateUI();
    }

    public void UpdateUI()
    {
        for (int i = 0; i < slotUIs.Length; i++)
        {
            slotUIs[i].UpdateSlot(inventory.slots[i]);
            Debug.Log("UI Updated");
        }
        
    }
}