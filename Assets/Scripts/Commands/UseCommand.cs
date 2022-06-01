using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseCommand : Command
{
    public BaseItem item;

    public UseCommand (UnitController owner, BaseItem item) : base(owner)
    {
        this.item = item;
    }

	public override CommandResult perform()
	{
        Debug.Log("Use Item");

        item.Use();

		return new CommandResult(CommandResult.CommandState.Succeeded, null);
	}
}
