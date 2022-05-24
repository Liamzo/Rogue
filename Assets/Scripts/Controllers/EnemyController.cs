using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : UnitController {
    public Enemy enemyType;

    public Object targetObject;

    public override void Turn() {
		base.Turn();

        enemyType.Controls(this);
    }

    public List<Vector2Int> FindPathToTarget() {
        if (targetObject != null) {
            return game.map.FindPath(new Vector2Int(this.x, this.y), new Vector2Int(targetObject.x,targetObject.y));
        }

        return null;
    }

     public void FindTarget() {
        Object target  = FindObjectOfType<PlayerController>();

        if (target == null) {
            targetObject = null;
            return;
        }

        int dx = target.x - x;
        int dy = target.y - y;

        float dist = Mathf.Sqrt((dx * dx) + (dy * dy));

        if (dist < unitStats.stats[(int) Stats.Sight].GetValue()) {
            targetObject = FindObjectOfType<PlayerController>();
            return;
        } else {
            targetObject = null;
            return;
        }
    }
}
