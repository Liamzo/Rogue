using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
public class Equipment : Item {
    public EquipmentSlot equipSlot;

    //public int armourModifier;
    //public int damageModifier;
    public List<StatValue> stats;

	public override void Use() {
		base.Use();
	}
}

public enum EquipmentSlot { Head, Body, Extra, Melee, Ranged }