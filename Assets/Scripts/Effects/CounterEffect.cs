using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CounterEffect : BaseEffect
{
    int timer;

    public CounterEffect(UnitController owner, int duration) {
		this.timer = duration;
		this.owner = owner;

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

        damage.attacker.unitStats.TakeDamge(new Damage(owner, damage.damage));
        
		damage.damage = 0;
	}
}
