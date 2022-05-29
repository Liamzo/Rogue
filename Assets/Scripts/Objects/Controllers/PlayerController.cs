using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Moveable))]
[RequireComponent(typeof(Vision))]
public class PlayerController : UnitController
{
	// Start is called before the first frame update
	protected PlayerSkills playerSkills;
	protected Moveable moveable;

	public event System.Action TargetUnitChange;

	// For FOV
	List<Vector2Int> visibleTiles = new List<Vector2Int>();

    public enum State {
		Controls,
		Range,
        Skill
	};
	public State state;

	protected override void Start() {
		base.Start();

        state = State.Controls;

		//equipmentManager.onEquipmentChanged += OnEquipmentChanged;

        unitStats = (PlayerStats) unitStats;
		this.playerSkills = (PlayerSkills) base.unitSkills;

        moveable = GetComponent<Moveable>();
	}

	new public bool Turn() {
        if (turn == false) {
            TurnEnd();
            return true;
        }

        if (state == State.Controls) {
            if (Controls()) {
                TurnEnd();

                return true;
            }
        } else if (state == State.Range) {
            if (RangedCheck()) {
                TurnEnd();

                return true;
            }
        } else if (state == State.Skill) {
            if (SkillCheck()) {
                TurnEnd();

                return true;
            }
        }

        return false;
	}

    public override void TurnEnd() {
        base.TurnEnd();

        state = State.Controls;
    }
    
	protected bool Controls() {
		if (Input.GetMouseButtonDown(0)) {
			if (EventSystem.current.IsPointerOverGameObject()) {
				return false;
			}
		 	Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		 	Vector2Int tile = game.map.GetXY(worldPosition);

			if (!game.map.IsPositionClear(tile, out Object blocked)) {
				if (blocked is EnemyController) {
                    ChangeTargetUnit((UnitController) blocked);
				}
			} else {
				ChangeTargetUnit(null);
			}
		}

		if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Keypad5)) {
			return true;
		}

		// Check for movement input
		if (CheckKeyboardMovement()) {
			return true;
		}// else if (CheckMouseMovement()) {
		// 	return;
		// }

		if (CheckSkillInput()) {
			return false;
		}

		if (Input.GetKeyDown(KeyCode.R)) {
			if (equipmentManager.GetRangedWeapon() != null) {
				state = State.Range;
				return false;
			}
		}

		if (Input.GetKeyDown(KeyCode.G)) {
			if (GetItem()) {
				return true;
			}
		}

		if (Input.GetButtonDown("Inventory")) {
			Inventory.instance.ToggleInventory();
		}

		if (Input.GetButtonDown("Skills")) {
			playerSkills.ToggleSkills();
		}

		if (Input.GetKeyDown(KeyCode.U)) {
			equipmentManager.UnequipAll();
		}

        return false;
	}


	bool CheckKeyboardMovement() {
		// Check for movement input
		if (Input.GetKeyDown(KeyCode.Keypad4)) {
			return MoveOrAct(new Vector2Int(x-1, y)); //Left
		} else if (Input.GetKeyDown(KeyCode.Keypad6)) {
			return MoveOrAct(new Vector2Int(x+1, y)); // Right
		} else if (Input.GetKeyDown(KeyCode.Keypad8)) {
			return MoveOrAct(new Vector2Int(x, y+1)); // Up
		} else if (Input.GetKeyDown(KeyCode.Keypad2)) {
			return MoveOrAct(new Vector2Int(x, y-1)); // Down
		} else if (Input.GetKeyDown(KeyCode.Keypad7)) {
			return MoveOrAct(new Vector2Int(x-1, y+1)); // Up Left
		} else if (Input.GetKeyDown(KeyCode.Keypad9)) {
			return MoveOrAct(new Vector2Int(x+1, y+1)); // Up Right
		} else if (Input.GetKeyDown(KeyCode.Keypad1)) {
			return MoveOrAct(new Vector2Int(x-1, y-1)); // Down Left
		} else if (Input.GetKeyDown(KeyCode.Keypad3)) {
			return MoveOrAct(new Vector2Int(x+1, y-1)); // Down Right
		}

		return false;
	}

	bool MoveOrAct(Vector2Int position) {
		Object found;

		if (game.map.IsPositionClear(position, out found)) {
            moveable.BaseMove(position.x, position.y);
            moveable.isMoving = true;

			return true;
		} 
		
		if (found != null) {
			if (found is EnemyController) {
				// Attack
				//state = State.Attacking;

                ChangeTargetUnit((EnemyController)found);
                equipmentManager.GetMainWeapon().Attack((EnemyController)found);
				return true;

			} else if (found is Interactable) {
				Interactable interactable = (Interactable) found;
				interactable.interact();
				return true;
			}
		}

		return false;
	}


    bool RangedCheck() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
			state = State.Controls;
			return false;
		}
		
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int targetCoords = game.map.GetXY(worldPosition);

        int xDistance = targetCoords.x - x;
        int yDistance = targetCoords.y - y;
        int max = Mathf.Max(Mathf.Abs(xDistance), Mathf.Abs(yDistance));

        float xStep = xDistance / (float) max;
        float yStep = yDistance / (float) max;

        List<Vector2Int> path = new List<Vector2Int>();

        for (int i = 1; i <= ((RangedWeapon)equipmentManager.GetRangedWeapon().item).range; i++) {
            int xPos = Mathf.RoundToInt(xStep * i) + x;
            int yPos = Mathf.RoundToInt(yStep * i) + y;

            Vector2Int tPos = new Vector2Int(xPos,yPos);

            if (!game.map.IsWithinMap(tPos)) {
                break;
            }

            path.Add(tPos);

            if (tPos == targetCoords) {
                break;
            }
        }

        foreach(Vector2Int tPos in path) {
            game.highlightedTiles.Add(game.map.GetTile(tPos.x,tPos.y));
        }

        if (Input.GetMouseButtonDown(0)) {
            Object found;
		    game.map.IsPositionClear(new Vector2Int(targetCoords.x, targetCoords.y), out found);
            if (found != null) {
                if (found is EnemyController) {
                    ChangeTargetUnit((UnitController)found);
                }
            }

            equipmentManager.GetRangedWeapon().Attack(new Vector2Int(targetCoords.x, targetCoords.y));

            return true;
        }
        return false;
        
    }


	bool CheckSkillInput() {
		BaseSkill skill = playerSkills.CheckSkillInput();

		if (skill != null) {
			usedSkill = skill;
			state = State.Skill;
			skill.skill.Activate(skill);
			return true;
		}

		return false;
	}

    bool SkillCheck() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
			state = State.Controls;
			return false;
		}

        if (usedSkill.Use()) {
			unitStats.AddOrRemoveGrace(-usedSkill.skill.graceCost);
			usedSkill = null;
	
			return true;
		}

        return false;
    }
    

	bool GetItem() {
		BaseItem item = game.GetItem(x,y);

		if (item == null) {
			return false;
		}

		item.PickUp(this);

		return true;
	}


	// Util

	public bool IsPointerOverGameObject() {
		return EventSystem.current.IsPointerOverGameObject();		    
	}

    public override void ChangeTargetUnit(UnitController unit) {
        base.ChangeTargetUnit(unit);
		TargetUnitChange();
    }

    public void KilledEnemy() {
        unitStats.AddOrRemoveGrace(1);
    }
}
