using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatEffect : BaseEffect
{
	int timer;
	StatValue sv;

	public StatEffect(StatValue sv, UnitController owner, int duration) {
		this.timer = duration;
		this.sv = sv;
		this.owner = owner;

		owner.unitStats.stats[(int)sv.stat].AddModifier(sv.value);

		//Debug.Log("Created StatEffect");

		owner.OnTurnStart += OnTurn;
	}

	public override void OnEnd() {
		owner.unitStats.stats[(int)sv.stat].RemoveModifier(sv.value);
		owner.OnTurnStart -= OnTurn;
	}

	public void OnTurn() {
		timer--;

		if (timer == 0) {
			OnEnd();
		}
	}
}
