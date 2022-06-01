using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCommand : Command
{
    UnitController target;

    public AttackCommand (UnitController owner, UnitController target) : base(owner)
    {
        this.target = target;
    }

	public override CommandResult perform()
	{
        Debug.Log("Attack");

        owner.equipmentManager.GetMainWeapon().Attack(target);

		return new CommandResult(CommandResult.CommandState.Succeeded, null);
	}
}
