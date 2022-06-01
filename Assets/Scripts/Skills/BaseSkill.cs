using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSkill {
    public Game game;

	public UnitController owner;

	public Skill skill;
	public KeyCode hotKey;

	// For Flowing Strike, need a better way to structure this
	public List<Vector2Int> openTargerts;
	public List<UnitController> closedTargerts;

    public BaseSkill (Skill skill) {
		this.game = Game.instance;
		this.skill = skill;

		openTargerts = new List<Vector2Int>();
		closedTargerts = new List<UnitController>();
	}

	public void OnUnlock () {
		skill.OnUnlock();
	}

	public CommandResult Use () {
		// Do something here, CHECK FOR ESCAPE!!!
		// Also move Mana Cost deduction here
		if (Input.GetKeyDown(KeyCode.Escape)) {
			return new CommandResult(CommandResult.CommandState.Failed, null);
		}

        CommandResult done = skill.Use(this);
		if (done.state == CommandResult.CommandState.Succeeded) {
            Reset();
			owner.unitStats.AddOrRemoveGrace(-skill.graceCost);
        }
        return done;
    }

	public virtual void Reset() {
		openTargerts.Clear();
    }

    public void Effects () {
        skill.Effects();
    }

	public bool CanBeActivated() {
		if (owner.unitStats.currentGrace >= skill.graceCost && skill.CanActivate(this)) {
			return true;
		}

		return false;
	}
}
