using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCommand : Command
{
    UnitController target;

    public AttackCommand (UnitController owner, UnitController target, float animationDelay = 0f) : base(owner, animationDelay)
    {
        this.target = target;
    }

	public override CommandResult perform()
	{
        if (waiting == true) {
            return CheckWait();
        }

        owner.equipmentManager.GetMeleeWeapon().Attack(target);

		if (animationDelay > 0f) {
            waiting = true;
            return new CommandResult(CommandResult.CommandState.Pending, null);
        } else {
            return new CommandResult(CommandResult.CommandState.Succeeded, null);
        }
	}
}
