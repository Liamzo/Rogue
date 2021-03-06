using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New BoS", menuName = "Skills/BoS")]
public class BurstStrength : Skill
{
    public StatValue sv;
    public int duration;

    public override CommandResult Use (BaseSkill baseSkill) {
        BaseEffect effect = new StatEffect(sv, baseSkill.owner, duration);

        return new CommandResult(CommandResult.CommandState.Succeeded, null);
    }
}
