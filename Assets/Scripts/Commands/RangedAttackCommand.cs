using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackCommand : Command
{
    Tile target;
    BaseRangedWeapon rangedWeapon;

    public RangedAttackCommand (UnitController owner, Tile target, BaseRangedWeapon rangedWeapon) : base(owner)
    {
        this.target = target;
        this.rangedWeapon = rangedWeapon;
    }

	public override CommandResult perform()
	{
        if (Input.GetKeyDown(KeyCode.Escape)) {
			return new CommandResult(CommandResult.CommandState.Failed, null);
		}

        // Check for ammo
        if (rangedWeapon.ammo < rangedWeapon.item.ammoCost) {
            // Return a reload command
            return new CommandResult(CommandResult.CommandState.Alternative, new ReloadCommand(owner, rangedWeapon));
        }

        Tile tempTarget = null;
        if (target != null) {
            tempTarget = target;
        }
        if (tempTarget == null) {
            tempTarget = rangedWeapon.Aim();
        }

        if (tempTarget != null) {
            rangedWeapon.Attack(tempTarget);
            return new CommandResult(CommandResult.CommandState.Succeeded, null);
        }

		if (target != null) {
            // Failed with given target, so fail rather than check for input
            return new CommandResult(CommandResult.CommandState.Failed, null);
        }
        return new CommandResult(CommandResult.CommandState.Pending, null);
	}
}
