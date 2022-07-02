using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadCommand : Command
{
    BaseRangedWeapon rangedWeapon;

    public ReloadCommand (UnitController owner, BaseRangedWeapon rangedWeapon) : base(owner)
    {
        this.rangedWeapon = rangedWeapon;
    }

	public override CommandResult perform()
	{
        rangedWeapon.Reload();

        return new CommandResult(CommandResult.CommandState.Succeeded, null);  
	}
}
