using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ranged Skill", menuName = "Skills/ranged skill")]
public class SkillRanged : Skill
{
	public int ammoCost;

	public override bool CanActivate (BaseSkill baseSkill) {
		if (base.CanActivate(baseSkill) == false) {
			return false;
		}

		if (baseSkill.owner.equipmentManager.GetRangedWeapon().ammo < ammoCost) {
			return false;
		}

		return true;
	}
}