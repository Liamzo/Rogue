using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCommand : Command
{
    BaseSkill skill;

    public SkillCommand (UnitController owner, BaseSkill skill, float animationDelay = 0f) : base(owner, animationDelay)
    {
        this.skill = skill;
    }

	public override CommandResult perform()
	{
        if (waiting == true) {
            return CheckWait();
        }

		CommandResult result = skill.Use();

        if (result.state == CommandResult.CommandState.Succeeded) {
            if (animationDelay > 0f) {
                waiting = true;
                return new CommandResult(CommandResult.CommandState.Pending, null);
            } else {
                return new CommandResult(CommandResult.CommandState.Succeeded, null);
            }
        }

        return result;
	}
}

