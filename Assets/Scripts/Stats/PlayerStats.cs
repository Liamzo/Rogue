using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : UnitStats {
	// Start is called before the first frame update
    EquipmentManager equipmentManager;
    
	void Start() {
        equipmentManager = GetComponent<EquipmentManager>();
        equipmentManager.onEquipmentChanged += OnEquipmentChanged;
	}

	void OnEquipmentChanged (BaseEquipment newItem, BaseEquipment oldItem) {
        if (newItem != null) {
            //armour.AddModifier(newItem.item.armourModifier);
            //-damage.AddModifier(newItem.item.damageModifier);

            // Set stats using item
            foreach (StatValue sv in newItem.item.stats) {
                int slot = (int) sv.stat;

                if (stats[slot] == null) {
                    Stat stat = new Stat();
                    stat.SetBaseValue(0);
                    stat.AddModifier(sv.value);
                    stats[slot] = stat;
                } else {
                    stats[slot].AddModifier(sv.value);
                }
            }
        }

        if (oldItem != null) {
            //armour.RemoveModifier(oldItem.item.armourModifier);
            //damage.RemoveModifier(oldItem.item.damageModifier);

            // Set stats using item
            foreach (StatValue sv in oldItem.item.stats) {
                int slot = (int) sv.stat;

                if (stats[slot] == null) {
                    Debug.LogError("Removing modifier from Stat which doesn't exisit");
                    continue;
                } else {
                    stats[slot].RemoveModifier(sv.value);
                }
            }
        }
    }



    public override void Die() {
        // This should be overwritten
        Debug.Log(transform.name + " died");
    }
}
