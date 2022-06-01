using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnequipCommand : Command
{
    public UnequipCommand (UnitController owner) : base(owner)
    {
        
    }

	public override CommandResult perform()
	{
        Debug.Log("Unequip");

        owner.equipmentManager.UnequipAll();

		return new CommandResult(CommandResult.CommandState.Succeeded, null);
	}
}
