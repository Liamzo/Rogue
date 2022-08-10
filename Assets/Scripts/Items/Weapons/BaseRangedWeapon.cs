using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseRangedWeapon : BaseWeapon {
    public new RangedWeapon item;
    public int ammo;

    public event System.Action OnAmmoChange;

    public BaseRangedWeapon (GameObject itemGOPrefab, RangedWeapon item, int x, int y, EquipmentManager equipmentManager = null) : base(itemGOPrefab, item, x, y, equipmentManager) {
        this.item = item;
        this.ammo = item.ammoCapacity;
	}

    public Tile Aim() {
        return item.Aim(this);
    }

    public void Attack(Tile target) {
        ((RangedWeapon)item).Attack(this, target, out UnitController targetUnit);
        owner.GetComponent<ActionManager>().SetAimPos(new Vector2Int(target.x, target.y));
        AudioManager.instance.PlaySoundOnce(owner.gameObject, item.soundType);

        ammo -= item.ammoCost;
        if (OnAmmoChange != null) {
            OnAmmoChange();
        }

        owner.CallOnAttackEnd(targetUnit, this);
        
        Reset();
    }

    public void Reload() {
        ammo = item.ammoCapacity;
        if (OnAmmoChange != null) {
            OnAmmoChange();
        }
    }
}
