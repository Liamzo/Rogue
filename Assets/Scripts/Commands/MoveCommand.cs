using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCommand : Command
{
    int x;
    int y;

    public MoveCommand (UnitController owner, int x, int y) : base(owner)
    {
        this.x = x;
        this.y = y;
    }

	public override CommandResult perform()
	{
        Debug.Log("Move");

        Object found;

		if (Game.instance.map.IsPositionClear(new Vector2Int(x, y), out found)) {
            owner.GetComponent<Moveable>().BaseMove(x, y);
		    owner.GetComponent<Moveable>().isMoving = true;

			return new CommandResult(CommandResult.CommandState.Succeeded, null);
		} 
		
		if (found != null) {
			if (found is UnitController) {
				return new CommandResult(CommandResult.CommandState.Alternative, new AttackCommand(owner, (UnitController)found));

			} else if (found is Interactable) {
                return new CommandResult(CommandResult.CommandState.Alternative, new InteractCommand(owner, (Interactable)found));
			}
		}

		return new CommandResult(CommandResult.CommandState.Failed, null);
	}
}
