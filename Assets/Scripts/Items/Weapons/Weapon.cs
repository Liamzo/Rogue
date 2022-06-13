using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Weapon")]
public class Weapon : Equipment {
    public WeaponType weaponType;
    public int range;

    public virtual void Attack(BaseWeapon baseWeapon, UnitController target, out bool killed) {
        killed = false;
        
        if (equipSlot == EquipmentSlot.Melee) {
            target.unitStats.TakeDamge(new Damage(baseWeapon.owner, baseWeapon.owner.unitStats.stats[(int)Stats.Strength].GetValue() + baseWeapon.owner.unitStats.stats[(int)Stats.MeleeDamage].GetValue()));
        } else if (equipSlot == EquipmentSlot.Ranged) {
            target.unitStats.TakeDamge(new Damage(baseWeapon.owner, baseWeapon.owner.unitStats.stats[(int)Stats.Perception].GetValue() + baseWeapon.owner.unitStats.stats[(int)Stats.RangedDamge].GetValue()));
        }

        if (target.unitStats.currentGrit <= 0) {
            killed = true;
        }
    }
}

[System.Serializable]
public enum WeaponType { None, Daggers, Sword, GreatSword, Maul, Pistol, Shotgun, Auto, Rifle }