using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCommand : Command
{
    BaseSkill skill;

    public SkillCommand (UnitController owner, BaseSkill skill) : base(owner)
    {
        this.skill = skill;
    }

	public override CommandResult perform()
	{
		return skill.Use();
	}
}

