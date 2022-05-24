using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseRangedWeapon : BaseWeapon {
    public BaseRangedWeapon (GameObject itemGOPrefab, RangedWeapon item, int x, int y) : base(itemGOPrefab, item, x, y) {
		this.item = item;
	}

    public void Attack(Vector2Int target) {
        ((RangedWeapon)item).Attack(this, target, out bool killed);
        Reset();
    }
}
