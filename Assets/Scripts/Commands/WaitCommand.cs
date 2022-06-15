using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitCommand : Command
{
    public WaitCommand (UnitController owner, float animationDelay = 0f) : base(owner, animationDelay)
    {
        
    }

	public override CommandResult perform()
	{
		return base.perform();
	}
}
