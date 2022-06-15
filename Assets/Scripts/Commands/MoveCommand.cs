using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCommand : Command
{
    int x;
    int y;
	float attackDelay;

    public MoveCommand (UnitController owner, int x, int y, float animationDelay = 0f, float attackDelay = 0f) : base(owner, animationDelay)
    {
        this.x = x;
        this.y = y;
		this.attackDelay = attackDelay;
    }

	public override CommandResult perform()
	{
		if (waiting == true) {
            return CheckWait();
        }

        Object found;

		if (Game.instance.map.IsPositionClear(new Vector2Int(x, y), out found)) {
            owner.GetComponent<Moveable>().BaseMove(x, y);

			if (animationDelay > 0f) {
				waiting = true;
				return new CommandResult(CommandResult.CommandState.Pending, null);
			} else {
				return new CommandResult(CommandResult.CommandState.Succeeded, null);
			}
		} 
		
		if (found != null) {
			if (found is UnitController) {
				return new CommandResult(CommandResult.CommandState.Alternative, new AttackCommand(owner, (UnitController)found, attackDelay));

			} else if (found is Interactable) {
                return new CommandResult(CommandResult.CommandState.Alternative, new InteractCommand(owner, (Interactable)found, animationDelay));
			}
		}

		return new CommandResult(CommandResult.CommandState.Failed, null);
	}
}
