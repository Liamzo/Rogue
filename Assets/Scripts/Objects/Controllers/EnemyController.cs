using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Moveable))]
public class EnemyController : UnitController {
    public Enemy enemyType;
    public Moveable moveable;

    protected override void Start() {
        GetComponent<UnitSkills>().baseUnitSkills = enemyType.skills;

		base.Start();

		moveable = GetComponent<Moveable>();
	}

    public override Command Turn() {
		TurnStart();

        if (turn == false) {
            queuedCommand = null;
            return new Command(this);
        }

        Command c = queuedCommand;
        queuedCommand = null;
        if (c == null) {
            c = enemyType.Controls(this);
        }

        return c;
    }
}
