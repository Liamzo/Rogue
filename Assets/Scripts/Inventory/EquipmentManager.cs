using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    // #region Singleton
    // public static EquipmentManager instance;
    
    // void Awake () {
    //     if (instance != null) {
    //         Debug.LogWarning("More than one EquipmentManager");
    //         return;
    //     }
    //     instance = this;
    // }
    // #endregion

    Inventory inventory;
    public UnitController unitController;

    public BaseEquipment[] currentEquipment;

    public DefaultEquipment defaultEquipmentData;
    //public BaseEquipment[] defaultEquipment; Soon

    public BaseWeapon defaultMeleeWeapon;

    public delegate void OnEquipmentChanged (BaseEquipment newItem, BaseEquipment oldItem);
    public OnEquipmentChanged onEquipmentChanged;

    void Start () {
        inventory = Inventory.instance;

        int numSlots = System.Enum.GetNames(typeof(EquipmentSlot)).Length;
        currentEquipment = new BaseEquipment[numSlots];
        //defaultEquipment = new BaseEquipment[numSlots]; Soon

        // Eventually, Loop for each given default equipment data and create a new BaseWeapon
    }

    public void SetDefaultEquipment() {
        defaultMeleeWeapon = new BaseWeapon(Game.instance.itemGOPrefab, defaultEquipmentData.defaultMeleeWeapon, unitController.x, unitController.y);
        defaultMeleeWeapon.owner = unitController;
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

    public BaseWeapon GetMainWeapon() {
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
