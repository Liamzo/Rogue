using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapon : BaseEquipment {
    public float xStep;
    public float yStep;

    public new Weapon item;

    public BaseWeapon (GameObject itemGOPrefab, Weapon item, int x, int y) : base(itemGOPrefab, item, x, y) {
		this.item = item;
	}

    public virtual void Attack(UnitController target) {
        owner.vision.ChangeTargetUnit(target);
        item.Attack(this, target, out bool killed);

        if (killed == true && owner is PlayerController) {
            owner.vision.ChangeTargetUnit(null);
            ((PlayerController)owner).KilledEnemy();
        }

        Reset();
    }

    protected virtual void Reset() {
        
    }
}
