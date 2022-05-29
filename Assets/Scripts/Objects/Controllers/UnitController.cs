using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitStats))]
[RequireComponent(typeof(EquipmentManager))]
[RequireComponent(typeof(UnitSkills))]
public class UnitController : Object
{
	public EquipmentManager equipmentManager;
	public UnitSkills unitSkills;
	public UnitStats unitStats;

	public string unitName;

    public int turnTime = 10;
	public int turnTimer;
	public bool turn = false;

	public enum UnitState {
		None,
		Moving,
		Attacking,
		Skilling
	};
	public UnitState unitState;

	public BaseSkill usedSkill;

	public event System.Action OnTurnStart = delegate { };
	public event System.Action OnTurnEnd = delegate { };

	public UnitController targetUnit;

	// Start is called before the first frame update
	protected override void Start() {
		base.Start();

		equipmentManager = GetComponent<EquipmentManager>();
		unitSkills = GetComponent<UnitSkills>();
		unitStats = GetComponent<UnitStats>();
		
		equipmentManager.unitController = this;
		unitSkills.unitController = this;
		//unitStats.unitController = this; // not needed atm

		equipmentManager.SetDefaultEquipment();

		unitState = UnitState.None;

        turnTimer = turnTime;
	}

    // Update is called once per frame
    protected virtual void Update() {
		if (unitState == UnitState.Moving) {
			Moving();
		}
	}

	public virtual void TurnStart() {
		turn = true;
		OnTurnStart();
		if (unitStats.currentGrace < unitStats.stats[(int)Stats.Grace].GetValue()) {
			unitStats.AddOrRemoveGrace(1);
		}
		turnTimer = turnTime;
	}


	public virtual void Turn() {
		Debug.Log("Unit turn");
    }

	public virtual void TurnEnd() {
		turn = false;
	}

	public virtual void ChangeTargetUnit(UnitController unit) {
        targetUnit = unit;
    }



	public override void Move(int x, int y) {
		float step =  10f * Time.deltaTime; // calculate distance to move
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(x,y), step);

        // Check if the position of the cube and sphere are approximately equal.
        if (Vector2.Distance(transform.position, new Vector2(x,y)) < 0.001f) {
            // Swap the position of the unit
			//base.Move(x, y);
			gameObject.transform.position = new Vector3 (x,y,0);

        	if (game.map.map[x,y].visible == false) {
				spriteRenderer.enabled = false;
			} else if (game.map.map[x,y].visible == true) {
				spriteRenderer.enabled = true;
			}
        }
	}


	protected virtual void Moving() {
		Move(x, y);

		if (transform.position.x == x && transform.position.y == y) {
			// Finished moving
			unitState = UnitState.None;
		}
	}

	// protected virtual void Attacking () {
	// 	if (Input.GetKeyDown(KeyCode.Escape)) {
	// 		state = State.Controls;
	// 		return;
	// 	}
		
	// 	if (attackWeapon == null) {
	// 		state = State.Controls;
	// 		return;
	// 	}

	// 	if (attackWeapon.Attack()) {
	// 		attackWeapon = null;
	// 		TurnEnd();
	// 		return;
	// 	}
	// }

	// protected virtual void Skilling () {
	// 	if (Input.GetKeyDown(KeyCode.Escape)) {
	// 		state = State.Controls;
	// 		return;
	// 	}

	// 	if (usedSkill == null) {
	// 		Debug.Log("No used skill");
	// 		state = State.Controls;
	// 		return;
	// 	}

	// 	if (usedSkill.Use()) {
	// 		unitStats.AddOrRemoveGrace(-usedSkill.skill.graceCost);

	// 		usedSkill = null;
	// 		TurnEnd();
	// 		return;
	// 	}
	// }
}
