﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Moveable))]
public class PlayerController : UnitController
{
	// Start is called before the first frame update
	protected PlayerSkills playerSkills;
	protected Moveable moveable;
	public PlayerVision playerVision;
	//public new PlayerVision vision;

    public enum State {
		Controls,
		Range,
        Skill
	};
	public State state;

	protected override void Start() {
		base.Start();

        state = State.Controls;

        unitStats = (PlayerStats) unitStats;
		this.playerSkills = (PlayerSkills) base.unitSkills;

        moveable = GetComponent<Moveable>();
		playerVision = (PlayerVision)vision;
	}

	public override Command Turn() {
        if (turn == false) {
            return new Command(this);
        }

        if (state == State.Controls) {
			Command c = Controls();
            if (c != null) {
                return c;
            }
        }

        return null;
	}

    public override void TurnEnd() {
        base.TurnEnd();

        state = State.Controls;
    }
    
	protected Command Controls() {
		playerVision.CheckTargetInput();

		if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Keypad5)) {
			return new WaitCommand(this);
		}

		// Check for movement input
		Command c;
		c = CheckKeyboardMovement();
		if (c != null) {
			return c;
		}// else if (CheckMouseMovement()) {
		// 	return;
		// }

		c = CheckSkillInput();
		if (c != null) {
			return c;
		}

		if (Input.GetKeyDown(KeyCode.R)) {
			if (Input.GetKeyDown(KeyCode.LeftControl)) {
				Debug.Log("boop");
			}
			if (equipmentManager.GetRangedWeapon() != null) {
				Tile target = null;
				if (vision.currentTarget != null) {
					target = game.map.GetTile(vision.currentTarget.x, vision.currentTarget.y);
				}
				return new RangedAttackCommand(this, target, equipmentManager.GetRangedWeapon());
			}
		}

		if (Input.GetKeyDown(KeyCode.G)) {
			return new PickUpCommand(this, x, y);
		}

		if (Input.GetButtonDown("Inventory")) {
			Inventory.instance.ToggleInventory();
		}
		if (Inventory.instance.usedItem != null) {
			BaseItem item = Inventory.instance.usedItem;
			Inventory.instance.usedItem = null;
			return new UseCommand(this, item);
		}

		if (Input.GetButtonDown("Skills")) {
			playerSkills.ToggleSkills();
		}

		if (Input.GetKeyDown(KeyCode.U)) {
			return new UnequipCommand(this);
		}

        return null;
	}


	Command CheckKeyboardMovement() {
		// Check for movement input
		if (Input.GetKeyDown(KeyCode.Keypad4)) {
			return new MoveCommand(this, x-1, y); //Left
		} else if (Input.GetKeyDown(KeyCode.Keypad6)) {
			return new MoveCommand(this, x+1, y); // Right
		} else if (Input.GetKeyDown(KeyCode.Keypad8)) {
			return new MoveCommand(this, x, y+1); // Up
		} else if (Input.GetKeyDown(KeyCode.Keypad2)) {
			return new MoveCommand(this, x, y-1); // Down
		} else if (Input.GetKeyDown(KeyCode.Keypad7)) {
			return new MoveCommand(this, x-1, y+1); // Up Left
		} else if (Input.GetKeyDown(KeyCode.Keypad9)) {
			return new MoveCommand(this, x+1, y+1); // Up Right
		} else if (Input.GetKeyDown(KeyCode.Keypad1)) {
			return new MoveCommand(this, x-1, y-1); // Down Left
		} else if (Input.GetKeyDown(KeyCode.Keypad3)) {
			return new MoveCommand(this, x+1, y-1); // Down Right
		}

		return null;
	}


	Command CheckSkillInput() {
		BaseSkill skill = playerSkills.CheckSkillInput();

		if (skill != null) {
			usedSkill = skill;
			if (vision.currentTarget != null) {
				usedSkill.target = game.map.GetTile(vision.currentTarget.x, vision.currentTarget.y);
			}
			skill.skill.Activate(usedSkill);
			return new SkillCommand(this, skill);
		}

		return null;
	}

	// Util

	public bool IsPointerOverGameObject() {
		return EventSystem.current.IsPointerOverGameObject();		    
	}

    // public override void ChangeTargetUnit(UnitController unit) {
    //     base.ChangeTargetUnit(unit);
	// 	TargetUnitChange();
    // }

    public void KilledEnemy() {
        unitStats.AddOrRemoveGrace(1);
		playerVision.UpdateTargetUnit(-1);
    }
}
