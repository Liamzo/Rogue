using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    Inventory inventory;

    public Transform itemsParent;
    public GameObject inventoryUI;
    public UnitController player;

    InventorySlot[] inventorySlots;
    EquipmentInvSlot[] equipSlots;
    // Start is called before the first frame update
    void Start()
    {
        inventory = Inventory.instance;
        inventory.onItemChangedCallBack += UpdateInventoryUI;
        inventorySlots = itemsParent.GetComponentsInChildren<InventorySlot>();

        player.GetComponent<EquipmentManager>().onEquipmentChanged += UpdateEquipmentUI;
        equipSlots = itemsParent.GetComponentsInChildren<EquipmentInvSlot>();
    }

    void UpdateInventoryUI() {
        for (int i = 0; i < inventorySlots.Length; i++) {
            if (i < inventory.items.Count) {
                inventorySlots[i].AddItem(inventory.items[i]);
            } else {
                inventorySlots[i].ClearSlot();
            }
        }
    }

    void UpdateEquipmentUI (BaseEquipment newItem, BaseEquipment oldItem) {
        if (newItem == null) {
            equipSlots[(int)oldItem.item.equipSlot].ClearSlot();
        } else {
            equipSlots[(int)newItem.item.equipSlot].AddItem(newItem);
        }
    }

    public void ToggleInventory() {
        inventoryUI.SetActive(!inventoryUI.activeSelf);
    }
}
