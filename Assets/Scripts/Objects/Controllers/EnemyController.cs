using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : UnitController {
    public Enemy enemyType;
    public Moveable moveable;

    protected override void Start() {
		base.Start();

		moveable = GetComponent<Moveable>();
	}

    public override void Turn() {
		TurnStart();

        if (turn == false) {
            return;
        }

        enemyType.Controls(this);

        TurnEnd();
    }

    public List<Vector2Int> FindPathToTarget() {
        if (targetUnit != null) {
            return game.map.FindPath(new Vector2Int(this.x, this.y), new Vector2Int(targetUnit.x,targetUnit.y));
        }

        return null;
    }

     public void FindTarget() {
        Object target  = FindObjectOfType<PlayerController>();

        if (target == null) {
            targetUnit = null;
            return;
        }

        int dx = target.x - x;
        int dy = target.y - y;

        float dist = Mathf.Sqrt((dx * dx) + (dy * dy));

        if (dist < unitStats.stats[(int) Stats.Sight].GetValue()) {
            targetUnit = FindObjectOfType<PlayerController>();
            return;
        } else {
            targetUnit = null;
            return;
        }
    }
}
