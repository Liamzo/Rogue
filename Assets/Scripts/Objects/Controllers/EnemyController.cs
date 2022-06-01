using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Moveable))]
[RequireComponent(typeof(Vision))]
public class EnemyController : UnitController {
    public Enemy enemyType;
    public Moveable moveable;
    public Vision vision;

    protected override void Start() {
		base.Start();

		moveable = GetComponent<Moveable>();
		vision = GetComponent<Vision>();
	}

    public override Command Turn() {
		TurnStart();

        if (turn == false) {
            return new Command(this);
        }

        Command c = enemyType.Controls(this);

        return c;
    }
}
