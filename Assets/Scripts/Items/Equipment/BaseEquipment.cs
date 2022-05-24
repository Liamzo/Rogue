using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEquipment : BaseItem
{
    public new Equipment item;

    public EquipmentManager equipmentManager;

    public BaseEquipment (GameObject itemGOPrefab, Equipment item, int x, int y, EquipmentManager equipmentManager = null) : base(itemGOPrefab, item, x, y) {
		this.item = item;

        this.equipmentManager = equipmentManager;
	}

    public override void Use () {
        base.Use();

        // Equip then remove from Inventory
        equipmentManager.Equip(this);
        RemoveFromInventory();
    }

    public override void PickUp(UnitController pickedUpBy) {
        base.PickUp(pickedUpBy);

        if (itemGO == null) {
            equipmentManager = pickedUpBy.equipmentManager;
        }

    }
}
