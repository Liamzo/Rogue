using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractCommand : Command
{
    Interactable interactable;

    public InteractCommand (UnitController owner, Interactable interactable) : base(owner)
    {
        this.interactable = interactable;
    }

	public override CommandResult perform()
	{
        Debug.Log("Interact");

		interactable.interact();

		return new CommandResult(CommandResult.CommandState.Succeeded, null);
	}
}
