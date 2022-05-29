using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Skill", menuName = "Skills/skill")]
public class Skill : ScriptableObject
{
    public new string name = "New Skill";
    public Sprite icon = null;

    public Skill[] requiredSkills;
    public int graceCost;
    public WeaponType requiredWeapon;

    public virtual bool Use (BaseSkill baseSkill) {
        Debug.Log("Using " + name);

        return true;
    }

    public virtual void Effects () {
        // For timed effects
    }

    public virtual void OnUnlock () {
		Debug.Log("Unlocked " + name);
	}

    public virtual bool CanActivate (BaseSkill baseSkill) {
        // bool canActivate = true;

        // if (requiredWeapon != baseSkill.owner.equipmentManager.GetMainWeapon().item.weaponType && requiredWeapon != WeaponType.None) {
        //     canActivate = false;
        // }

        Debug.Log(baseSkill.owner.equipmentManager.GetMainWeapon());
        Debug.Log(baseSkill.owner.equipmentManager.GetRangedWeapon());
        if ( (requiredWeapon == baseSkill.owner.equipmentManager.GetMainWeapon().item.weaponType) || (requiredWeapon == baseSkill.owner.equipmentManager.GetRangedWeapon().item.weaponType) || (requiredWeapon == WeaponType.None) ) {
            return true;
        }

		return false;
	}

    public virtual void Activate(BaseSkill baseSkill) {
        Debug.Log("Activated " + name);
    }
}
