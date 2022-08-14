using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New FastHands", menuName = "Skills/Pistol/FastHands")]
public class FastHands : SkillRanged
{
    public override void OnUnlock (BaseSkill baseSkill) {
        base.OnUnlock(baseSkill);
        // Shoot on melee attack if target not dead
        new FastHandsEffect(baseSkill.owner);
	}


    public override CommandResult Use (BaseSkill baseSkill) {
        return new CommandResult(CommandResult.CommandState.Failed, null);
    }
}
