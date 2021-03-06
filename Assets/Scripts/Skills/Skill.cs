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

    public int coolDown;

    public virtual CommandResult Use (BaseSkill baseSkill) {
        //Debug.Log("Using " + name);

        return new CommandResult(CommandResult.CommandState.Succeeded, null);
    }

    public virtual void Effects () {
        // For timed effects
    }

    public virtual void OnUnlock () {
        Logger.instance.AddLog("Learned " + name);
	}

    public virtual bool CanActivate (BaseSkill baseSkill) {
        if (requiredWeapon == WeaponType.None) {
            return true;
        }

        if (baseSkill.owner.equipmentManager.GetMeleeWeapon() != null) {
            if (requiredWeapon == baseSkill.owner.equipmentManager.GetMeleeWeapon().item.weaponType) {
                return true;
            }
        }

        if (baseSkill.owner.equipmentManager.GetRangedWeapon() != null) {
            if (requiredWeapon == baseSkill.owner.equipmentManager.GetRangedWeapon().item.weaponType) {
                return true;
            }
        }

		return false;
	}

    public virtual void Activate(BaseSkill baseSkill) {
        //Debug.Log("Activated " + name);
    }
}
