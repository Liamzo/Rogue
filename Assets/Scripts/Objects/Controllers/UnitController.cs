using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitStats))]
[RequireComponent(typeof(EquipmentManager))]
[RequireComponent(typeof(UnitSkills))]
[RequireComponent(typeof(Vision))]
public class UnitController : Object
{
	public EquipmentManager equipmentManager;
	public UnitSkills unitSkills;
	public UnitStats unitStats;
	public Vision vision;

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

	public Command queuedCommand;

	public event System.Action OnTurnStart = delegate { };
	public event System.Action OnTurnEnd = delegate { };

	// Start is called before the first frame update
	protected override void Start() {
		base.Start();

		equipmentManager = GetComponent<EquipmentManager>();
		unitSkills = GetComponent<UnitSkills>();
		unitStats = GetComponent<UnitStats>();
		vision = GetComponent<Vision>();
		
		equipmentManager.unitController = this;
		unitSkills.unitController = this;
		//unitStats.unitController = this; // not needed atm

		equipmentManager.SetDefaultEquipment();
		equipmentManager.SetStartingEquipment();
		unitSkills.SetDefaultSkills();

		unitState = UnitState.None;

        turnTimer = turnTime;
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
		//Debug.Log("Unit turn");
		return new Command(this);
    }

	public virtual void TurnEnd() {
		turn = false;
	}
}
