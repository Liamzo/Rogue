using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    Inventory inventory;
    public UnitController unitController;

    public BaseEquipment[] currentEquipment;

    public DefaultEquipment defaultEquipmentData;
    //public BaseEquipment[] defaultEquipment; Soon
    public StartingEquipment startingEquipment;

    public BaseWeapon defaultMeleeWeapon;

    public delegate void OnEquipmentChanged (BaseEquipment newItem, BaseEquipment oldItem);
    public OnEquipmentChanged onEquipmentChanged;

    public BaseEquipment toRemoveItem = null;

    void Start () {
        inventory = Inventory.instance;
    }

    public void SetDefaultEquipment() {
        // Eventually, Loop for each given default equipment data and create a new BaseWeapon
        // int numSlots = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
        // currentEquipment = new BaseEquipment[numSlots];
        //defaultEquipment = new BaseEquipment[numSlots]; Soon
        defaultMeleeWeapon = new BaseWeapon(Game.instance.itemGOPrefab, defaultEquipmentData.defaultMeleeWeapon, unitController.x, unitController.y);
        defaultMeleeWeapon.owner = unitController;
    }

    public void SetStartingEquipment() {
        int numSlots = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
        currentEquipment = new BaseEquipment[numSlots];

        if (startingEquipment == null) {
            return;
        }

        foreach (Equipment equipment in startingEquipment.equipment) {
            if (equipment.equipSlot == EquipmentSlot.Melee) {
                BaseWeapon baseWeapon = new BaseWeapon(Game.instance.itemGOPrefab, (Weapon)equipment, 0, 0, this);
                baseWeapon.owner = unitController;
                Equip(baseWeapon);
            } else if (equipment.equipSlot == EquipmentSlot.Ranged) {
                BaseRangedWeapon baseRangedWeapon = new BaseRangedWeapon(Game.instance.itemGOPrefab, (RangedWeapon)equipment, 0, 0, this);
                baseRangedWeapon.owner = unitController;
                Equip(baseRangedWeapon);
            } else {
                BaseEquipment baseEquipment = new BaseEquipment(Game.instance.itemGOPrefab, equipment, 0, 0, this);
                baseEquipment.owner = unitController;
                Equip(baseEquipment);
            }
        }
    }

    public void Equip (BaseEquipment newItem) {
        int slot = (int) newItem.item.equipSlot;

        BaseEquipment oldItem = null;

        if (currentEquipment[slot] != null) {
            oldItem = currentEquipment[slot];
            inventory.Add(oldItem);
        }

        if (onEquipmentChanged != null) {
            onEquipmentChanged.Invoke(newItem, oldItem);
        }

        currentEquipment[slot] = newItem;
    }

    public void Unequip(int slot) {
        if (currentEquipment[slot] != null) {
            BaseEquipment oldItem = currentEquipment[slot];
            inventory.Add(oldItem);

            currentEquipment[slot] = null;

            if (onEquipmentChanged != null) {
                onEquipmentChanged.Invoke(null, oldItem);
            }
        }
    }

    public void UnequipAll () {
        for (int i = 0; i < currentEquipment.Length; i++) {
            Unequip(i);
        }
    }


    public BaseEquipment GetEquipment(int slot) {
        return currentEquipment[slot];
    }

    public BaseWeapon GetMeleeWeapon() {
        //FIX
        if (currentEquipment == null) {
            return defaultMeleeWeapon;
        }
        
        BaseEquipment be = currentEquipment[(int)EquipmentSlot.Melee];

        if (be != null) {
            return (BaseWeapon) be;
        }

        return defaultMeleeWeapon;
    }

    public BaseRangedWeapon GetRangedWeapon() {
        if (currentEquipment == null) {
            return null;
        }
        
        BaseEquipment be = currentEquipment[(int)EquipmentSlot.Ranged];

        if (be != null) {
            return (BaseRangedWeapon) be;
        }

        return null;
    }

}
