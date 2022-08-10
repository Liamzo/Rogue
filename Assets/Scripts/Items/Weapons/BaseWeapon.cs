using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseWeapon : BaseEquipment {
    public new Weapon item;

    public BaseWeapon (GameObject itemGOPrefab, Weapon item, int x, int y, EquipmentManager equipmentManager = null) : base(itemGOPrefab, item, x, y, equipmentManager) {
        this.item = item;
	}

    public virtual void Attack(UnitController target) {
        owner.vision.ChangeTargetUnit(target);
        item.Attack(this, target);
        owner.GetComponent<ActionManager>().SetAimPos(new Vector2Int(target.x, target.y)); // Attack animation Bump
        AudioManager.instance.PlaySoundOnce(owner.gameObject, item.soundType);
        
        owner.CallOnAttackEnd(target, this);

        Reset();
    }

    protected virtual void Reset() {
        
    }
}
