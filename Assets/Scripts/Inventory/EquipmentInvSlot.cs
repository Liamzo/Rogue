using UnityEngine;
using UnityEngine.UI;

public class EquipmentInvSlot : MonoBehaviour
{
    public Image icon;
    public Sprite baseIcon;

    BaseEquipment equipment;

    public void AddItem (BaseEquipment newEquipment) {
        equipment = newEquipment;

        icon.sprite = equipment.item.icon;
    }

    public void ClearSlot () {
        equipment = null;

        icon.sprite = baseIcon;
    }

    public void UnequipItem () {
        if (equipment != null) {
            equipment.owner.equipmentManager.toRemoveItem = equipment;
        }
    }
}
