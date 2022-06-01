using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandResult
{
    public enum CommandState {
        Succeeded,
        Failed,
        Pending,
        Alternative
    }
    public CommandState state;
    public Command alternative;

    public CommandResult(CommandState state, Command alternative) {
        this.state = state;
        this.alternative = alternative;
    }
}
