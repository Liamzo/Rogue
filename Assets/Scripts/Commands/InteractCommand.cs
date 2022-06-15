using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractCommand : Command
{
    Interactable interactable;

    public InteractCommand (UnitController owner, Interactable interactable, float animationDelay = 0f) : base(owner, animationDelay)
    {
        this.interactable = interactable;
    }

	public override CommandResult perform()
	{
		interactable.interact();

		return new CommandResult(CommandResult.CommandState.Succeeded, null);
	}
}
