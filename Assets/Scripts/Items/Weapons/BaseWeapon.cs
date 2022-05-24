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
        item.Attack(this, target, out bool killed);
        Reset();
    }

    protected virtual void Reset() {
        
    }
}
