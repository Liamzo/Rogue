using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Daggers", menuName = "Inventory/Daggers")]
public class Daggers : Weapon
{
    public int offHandDamage;

    public override void Attack(BaseWeapon baseWeapon, UnitController target, out bool killed) {
        killed = false;
        BaseDaggers baseDaggers = (BaseDaggers) baseWeapon;
        
        //target.unitStats.TakeDamge(baseWeapon.owner.unitStats.stats[(int)Stats.Strength].GetValue() + baseWeapon.owner.unitStats.stats[(int)Stats.MeleeDamage].GetValue());
        target.unitStats.TakeDamge(new Damage(baseWeapon.owner, baseWeapon.owner.unitStats.stats[(int)Stats.Strength].GetValue() + baseWeapon.owner.unitStats.stats[(int)Stats.MeleeDamage].GetValue()));
        
        target.unitStats.TakeDamge(new Damage(baseWeapon.owner, offHandDamage));
            
        if (target.unitStats.currentGrit <= 0) {
            Debug.Log("Killed " + target.name);
            killed = true;
            //baseWeapon.owner.unitStats.AddOrRemoveGrace(1);
        }
        
    }
}
