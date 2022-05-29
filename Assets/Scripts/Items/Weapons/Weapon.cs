using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Weapon")]
public class Weapon : Equipment {
    public WeaponType weaponType;
    public int range;

    public virtual void Attack(BaseWeapon baseWeapon, UnitController target, out bool killed) {
        killed = false;
        Debug.Log("Attack");
    }
}

[System.Serializable]
public enum WeaponType { None, Daggers, Sword, GreatSword, Maul, Pistol, Shotgun, Auto, Rifle }