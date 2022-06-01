using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitCommand : Command
{
    public WaitCommand (UnitController owner) : base(owner)
    {
        
    }

	public override CommandResult perform()
	{
		return new CommandResult(CommandResult.CommandState.Succeeded, null);
	}
}
