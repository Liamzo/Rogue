using System.Collections;
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

	public event System.Action UpdateUI;

	protected override void Start() {
		base.Start();

        unitStats = (PlayerStats) unitStats;
		this.playerSkills = (PlayerSkills) base.unitSkills;

        moveable = GetComponent<Moveable>();
		playerVision = (PlayerVision)vision;
	}

	public override void TurnStart()
	{
		base.TurnStart();

		if (UpdateUI != null) {
			UpdateUI();
		}
	}

	public override Command Turn() {
        if (turn == false) {
			queuedCommand = null;
            return new Command(this);
        }

		Command c = queuedCommand;
        queuedCommand = null;
        if (c == null) {
            c = Controls();
        }
        
        return c;
	}

    public override void TurnEnd() {
        base.TurnEnd();
    }
    
	public Command Controls() {
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

		if (Input.GetKeyDown(KeyCode.F) && Input.GetKeyDown(KeyCode.LeftControl)) {
			Debug.Log("boop");
		}
		if (Input.GetKeyDown(KeyCode.F)) {
			if (equipmentManager.GetRangedWeapon() != null) {
				Tile target = null;
				if (vision.currentTarget != null) {
					target = game.map.GetTile(vision.currentTarget.x, vision.currentTarget.y);
				}
				return new RangedAttackCommand(this, target, equipmentManager.GetRangedWeapon());
			}
		}

		if (Input.GetKeyDown(KeyCode.R)) {
			if (equipmentManager.GetRangedWeapon() != null) {
				return new ReloadCommand(this, equipmentManager.GetRangedWeapon());
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

		if (equipmentManager.toRemoveItem != null) {
			BaseEquipment equipment = equipmentManager.toRemoveItem;
			equipmentManager.toRemoveItem = null;
			return new UnequipCommand(this, equipment);
		}

        return null;
	}


	Command CheckKeyboardMovement() {
		// Check for movement input
		if (Input.GetKeyDown(KeyCode.Keypad4)) {
			return new MoveCommand(this, x-1, y, 0f, 0.1f); //Left
		} else if (Input.GetKeyDown(KeyCode.Keypad6)) {
			return new MoveCommand(this, x+1, y, 0f, 0.1f); // Right
		} else if (Input.GetKeyDown(KeyCode.Keypad8)) {
			return new MoveCommand(this, x, y+1, 0f, 0.1f); // Up
		} else if (Input.GetKeyDown(KeyCode.Keypad2)) {
			return new MoveCommand(this, x, y-1, 0f, 0.1f); // Down
		} else if (Input.GetKeyDown(KeyCode.Keypad7)) {
			return new MoveCommand(this, x-1, y+1, 0f, 0.1f); // Up Left
		} else if (Input.GetKeyDown(KeyCode.Keypad9)) {
			return new MoveCommand(this, x+1, y+1, 0f, 0.1f); // Up Right
		} else if (Input.GetKeyDown(KeyCode.Keypad1)) {
			return new MoveCommand(this, x-1, y-1, 0f, 0.1f); // Down Left
		} else if (Input.GetKeyDown(KeyCode.Keypad3)) {
			return new MoveCommand(this, x+1, y-1, 0f, 0.1f); // Down Right
		}

		return null;
	}


	Command CheckSkillInput() {
		BaseSkill skill = playerSkills.CheckSkillInput();

		if (skill != null) {
			// Do we need used skill at all here?
			usedSkill = skill;
			if (vision.currentTarget != null) {
				usedSkill.target = game.map.GetTile(vision.currentTarget.x, vision.currentTarget.y);
			}
			skill.skill.Activate(usedSkill);
			return new SkillCommand(this, skill, 0.1f);
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
    }
}
