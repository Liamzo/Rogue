using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New FastHands", menuName = "Skills/Pistol/FastHands")]
public class FastHands : SkillRanged
{
    // Shoot on melee attack if target not dead
    public override CommandResult Use (BaseSkill baseSkill) {
        BaseEffect effect = new FastHandsEffect(baseSkill.owner);

        return new CommandResult(CommandResult.CommandState.Succeeded, null);
    }
}
