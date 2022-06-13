using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseRangedWeapon : BaseWeapon {
    public BaseRangedWeapon (GameObject itemGOPrefab, RangedWeapon item, int x, int y) : base(itemGOPrefab, item, x, y) {
		this.item = item;
	}

    public Tile Aim() {
        return ((RangedWeapon)item).Aim(this);
    }

    public void Attack(Tile target) {
        ((RangedWeapon)item).Attack(this, target, out bool killed);
        owner.GetComponent<ActionManager>().SetAimPos(new Vector2Int(target.x, target.y));

        if (killed == true && owner is PlayerController) {
            owner.vision.ChangeTargetUnit(null);
            ((PlayerController)owner).KilledEnemy();
        }
        
        Reset();
    }
}
