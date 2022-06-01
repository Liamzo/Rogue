using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedAttackCommand : Command
{
    Vector2Int target;
    BaseRangedWeapon rangedWeapon;

    public RangedAttackCommand (UnitController owner, Vector2Int target, BaseRangedWeapon rangedWeapon) : base(owner)
    {
        this.target = target;
        this.rangedWeapon = rangedWeapon;
    }

	public override CommandResult perform()
	{
        rangedWeapon.Attack(target);

		return new CommandResult(CommandResult.CommandState.Succeeded, null);
	}
}
