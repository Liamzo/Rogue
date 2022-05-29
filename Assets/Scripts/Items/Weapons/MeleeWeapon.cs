using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New MeleeWeapom", menuName = "Inventory/MeleeWeapom")]
public class MeleeWeapon : Weapon
{
    public override void Attack(BaseWeapon baseWeapon, UnitController target, out bool killed) {
        killed = false;
        //target.unitStats.TakeDamge(baseWeapon.owner.unitStats.stats[(int)Stats.Strength].GetValue() + baseWeapon.owner.unitStats.stats[(int)Stats.MeleeDamage].GetValue());
        target.unitStats.TakeDamge(new Damage(baseWeapon.owner, baseWeapon.owner.unitStats.stats[(int)Stats.Strength].GetValue() + baseWeapon.owner.unitStats.stats[(int)Stats.MeleeDamage].GetValue()));
        
        if (target.unitStats.currentGrit <= 0) {
            Debug.Log("Killed " + target.name);
            killed = true;
            //baseWeapon.owner.unitStats.AddOrRemoveGrace(1);
        }
    }
}
