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
		//Moving,
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

    protected virtual void Update() {

	}

	public virtual void TurnStart() {
		turn = true;
		OnTurnStart();
		if (unitStats.currentGrace < unitStats.stats[(int)Stats.Grace].GetValue()) {
			unitStats.AddOrRemoveGrace(1);
		}
		turnTimer = turnTime;
	}


	public virtual Command Turn() {
		Debug.Log("Unit turn");
		return new Command(this);
    }

	public virtual void TurnEnd() {
		turn = false;
	}

	public virtual void ChangeTargetUnit(UnitController unit) {
        targetUnit = unit;
    }

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
