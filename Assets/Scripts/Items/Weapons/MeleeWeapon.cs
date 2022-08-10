using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New MeleeWeapom", menuName = "Inventory/MeleeWeapom")]
public class MeleeWeapon : Weapon
{
    public override void Attack(BaseWeapon baseWeapon, UnitController target) {
        base.Attack(baseWeapon, target);
    }
}
