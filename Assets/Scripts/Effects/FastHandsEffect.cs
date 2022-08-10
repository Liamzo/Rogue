using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FastHandsEffect : BaseEffect
{
    public FastHandsEffect(UnitController owner) {
		this.owner = owner;

		owner.OnAttackEnd += OnAttackEnd;
	}

	public override void OnEnd() {
	}

	public void OnAttackEnd(UnitController target, BaseWeapon weapon) {
		// If target is alive, and used weapon was melee
		// Shoot with ranged

		if (weapon.item.equipSlot != EquipmentSlot.Melee || target.unitStats.currentGrit <= 0) {
			return;
		}

		BaseRangedWeapon rangedWeapon = owner.equipmentManager.GetRangedWeapon();

		if (rangedWeapon.ammo >= rangedWeapon.item.ammoCost) {
			owner.equipmentManager.GetRangedWeapon().Attack(Game.instance.map.GetTile(target.x, target.y));
		}
	}
}