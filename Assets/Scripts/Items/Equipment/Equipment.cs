using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/Equipment")]
public class Equipment : Item {
    public EquipmentSlot equipSlot;

    public List<StatValue> stats;

	public override void Use() {
		Logger.instance.AddLog("Equipped " + name);
	}
}

public enum EquipmentSlot { Head, Body, Extra, Melee, Ranged }