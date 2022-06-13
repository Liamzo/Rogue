using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapon : BaseEquipment {
    public new Weapon item;

    public BaseWeapon (GameObject itemGOPrefab, Weapon item, int x, int y) : base(itemGOPrefab, item, x, y) {
		this.item = item;
	}

    public virtual void Attack(UnitController target) {
        owner.vision.ChangeTargetUnit(target);
        item.Attack(this, target, out bool killed);
        owner.GetComponent<ActionManager>().SetAimPos(new Vector2Int(target.x, target.y)); // Attack animation Bump

        if (killed == true && owner is PlayerController) {
            owner.vision.ChangeTargetUnit(null);
            ((PlayerController)owner).KilledEnemy();
        }

        Reset();
    }

    protected virtual void Reset() {
        
    }
}
