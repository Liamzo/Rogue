using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Daggers", menuName = "Inventory/Daggers")]
public class Daggers : Weapon
{
    public int offHandDamage;

    public override void Attack(BaseWeapon baseWeapon, UnitController target) {
        BaseDaggers baseDaggers = (BaseDaggers) baseWeapon;
        
        target.unitStats.TakeDamge(new Damage(baseWeapon.owner, offHandDamage));

        base.Attack(baseWeapon, target);
    }
}
