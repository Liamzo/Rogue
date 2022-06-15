using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Command
{
    protected UnitController owner;
    protected float animationDelay;
    protected bool waiting;

    public Command (UnitController owner, float animationDelay = 0f) 
    {
        this.owner = owner;
        this.animationDelay = animationDelay;
    }

    public virtual CommandResult perform() {
        if (waiting == true) {
            return CheckWait();
        }

        if (animationDelay > 0f) {
            waiting = true;
            return new CommandResult(CommandResult.CommandState.Pending, null);
        }

        return new CommandResult(CommandResult.CommandState.Succeeded, null);
    }

    protected virtual CommandResult CheckWait() {
        animationDelay -= Time.deltaTime;

        if (animationDelay <= 0f) {
            animationDelay = 0;
            waiting = false;
            return new CommandResult(CommandResult.CommandState.Succeeded, null);
        }

        return new CommandResult(CommandResult.CommandState.Pending, null);
    }
}
