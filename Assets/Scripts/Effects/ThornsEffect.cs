using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThornsEffect : BaseEffect
{
    int timer;
	int power;

    public ThornsEffect(UnitController owner, int duration, int power) {
		this.timer = duration;
		this.owner = owner;
		this.power = power;

		owner.OnTurnStart += OnTurn;
		owner.unitStats.OnTakeDamage += OnTakeDamage;
	}

	public override void OnEnd() {
		owner.OnTurnStart -= OnTurn;
        owner.unitStats.OnTakeDamage -= OnTakeDamage;
	}

	public void OnTurn() {
		timer--;

		if (timer == 0) {
			OnEnd();
		}
	}

	public void OnTakeDamage(Damage damage) {
		if (damage.attacker == null) {
			return;
		}
        damage.attacker.unitStats.TakeDamge(new Damage(null, power));
	}
}