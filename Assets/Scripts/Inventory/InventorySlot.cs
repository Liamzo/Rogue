using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image icon;
    public Button removeButton;

    BaseItem item;

    public void AddItem (BaseItem newItem) {
        item = newItem;

        icon.sprite = item.item.icon;
        icon.enabled = true;
        removeButton.interactable = true;
    }

    public void ClearSlot () {
        item = null;

        icon.sprite = null;
        icon.enabled = false;
        removeButton.interactable = false;
    }

    public void OnRemoveButton() {
        item.Drop();
        Inventory.instance.Remove(item);
        
    }

    public void UseItem () {
        if (item != null) {
            item.Use();
        }
    }
}
