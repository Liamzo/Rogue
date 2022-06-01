using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseRangedWeapon : BaseWeapon {
    public BaseRangedWeapon (GameObject itemGOPrefab, RangedWeapon item, int x, int y) : base(itemGOPrefab, item, x, y) {
		this.item = item;
	}

    public Command Aim() {
        return ((RangedWeapon)item).Aim(this);
    }

    public void Attack(Vector2Int target) {
        ((RangedWeapon)item).Attack(this, target, out bool killed);

        if (killed == true && owner is PlayerController) {
            ((PlayerController)owner).KilledEnemy();
        }
        
        Reset();
    }
}
