using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dig Deep", menuName = "Skills/DigDeep")]
public class DigDeep : Skill {
    public override bool Use (BaseSkill baseSkill) {
        baseSkill.owner.unitStats.TakeTrueDamage(1);

        baseSkill.owner.unitStats.AddOrRemoveGrace(baseSkill.owner.unitStats.stats[(int)Stats.Grace].GetValue() * 2);

        return true;
    }

	public override bool CanActivate(BaseSkill baseSkill) {
        bool canActivate = base.CanActivate(baseSkill);

        // Can only be used on low Grace
        if (baseSkill.owner.unitStats.currentGrace > 1) {
            canActivate = false;
        }

		return canActivate;
	}

    
}
