using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New ThornArmour", menuName = "Skills/ThornArmour")]
public class ThornArmour : Skill
{
    public int power;
    public int duration;

    public override CommandResult Use (BaseSkill baseSkill) {
        BaseEffect effect = new ThornsEffect(baseSkill.owner, duration, power);

        return new CommandResult(CommandResult.CommandState.Succeeded, null);
    }
}
