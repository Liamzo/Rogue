using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseDaggers : BaseWeapon
{
    public BaseDaggers (GameObject itemGOPrefab, Weapon item, int x, int y) : base(itemGOPrefab, item, x, y) {

	}

    public int GetOffHandDamage () {
        Daggers daggers = (Daggers) item;
        return daggers.offHandDamage;
    }
}
