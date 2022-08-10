using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Weapon")]
public class Weapon : Equipment {
    public WeaponType weaponType;
    public AudioManager.Sound soundType;
    public int range;

    public virtual void Attack(BaseWeapon baseWeapon, UnitController target) {
        if (equipSlot == EquipmentSlot.Melee) {
            Damage damage = new Damage(baseWeapon.owner, baseWeapon.owner.unitStats.stats[(int)Stats.Strength].GetValue() + baseWeapon.owner.unitStats.stats[(int)Stats.MeleeDamage].GetValue());
            baseWeapon.owner.CallOnAttackStart(target, damage);
            target.unitStats.TakeDamge(damage);
        } else if (equipSlot == EquipmentSlot.Ranged) {
            Damage damage = new Damage(baseWeapon.owner, baseWeapon.owner.unitStats.stats[(int)Stats.Perception].GetValue() + baseWeapon.owner.unitStats.stats[(int)Stats.RangedDamge].GetValue());
            baseWeapon.owner.CallOnAttackStart(target, damage);
            target.unitStats.TakeDamge(damage);
        }

        if (baseWeapon.owner is PlayerController) {
            Game.instance.TriggerShake();
        }
    }
}

[System.Serializable]
public enum WeaponType { None, Daggers, Sword, GreatSword, Maul, Pistol, Shotgun, Auto, Rifle }