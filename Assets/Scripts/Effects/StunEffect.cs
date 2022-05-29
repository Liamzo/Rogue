using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunEffect : BaseEffect
{
    int timer;

    public StunEffect(UnitController owner, int duration) {
		this.timer = duration;
		this.owner = owner;

		owner.OnTurnStart += OnTurn;
	}

	public override void OnEnd() {
        owner.OnTurnStart -= OnTurn;
	}

	public void OnTurn() {
        owner.turn = false;

		timer--;

		if (timer == 0) {
			OnEnd();
		}
	}
}