using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseDaggers : BaseWeapon
{
    public int count;

    public BaseDaggers (GameObject itemGOPrefab, Weapon item, int x, int y) : base(itemGOPrefab, item, x, y) {
		count = 0;
	}

    protected override void Reset() {
        base.Reset();
        count = 0;
    }


    public int GetOffHandDamage () {
        Daggers daggers = (Daggers) item;
        return daggers.offHandDamage;
    }
}
