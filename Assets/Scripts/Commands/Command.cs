using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Command
{
    protected UnitController owner;

    public Command (UnitController owner) 
    {
        this.owner = owner;
    }

    public virtual CommandResult perform() {
        return new CommandResult(CommandResult.CommandState.Succeeded, null);
    }
}
