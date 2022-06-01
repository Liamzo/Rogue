using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpCommand : Command
{
    int x;
    int y;

    public PickUpCommand (UnitController owner, int x, int y) : base(owner)
    {
        this.x = x;
        this.y = y;
    }

	public override CommandResult perform()
	{
        Debug.Log("Pick Up");

		BaseItem item = Game.instance.GetItem(x,y);

		if (item == null) {
			return new CommandResult(CommandResult.CommandState.Failed, null);
		}

		item.PickUp(owner);

		return new CommandResult(CommandResult.CommandState.Succeeded, null);
	}
}
