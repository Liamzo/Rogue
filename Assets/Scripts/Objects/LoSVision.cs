using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoSVision : Vision
{
    public override UnitController FindTarget(UnitController tryTarget) {
        UnitController target  = tryTarget;

        if (target == null) {
            return null;
        }

        if (base.FindTarget(tryTarget) != null) {
            // Target in range
            int dx = target.x - parent.x;
            int dy = target.y - parent.y;
            int max = Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy));

            float xStep = dx / (float) max;
            float yStep = dy / (float) max;

            List<Vector2Int> path = new List<Vector2Int>();

            for (int i = 1; i <= parent.unitStats.stats[(int)Stats.Sight].GetValue(); i++) {
                int xPos = Mathf.RoundToInt(xStep * i) + parent.x;
                int yPos = Mathf.RoundToInt(yStep * i) + parent.y;

                Vector2Int tPos = new Vector2Int(xPos,yPos);

                if (!Game.instance.map.IsWithinMap(tPos)) {
                    return null;
                }

                if (Game.instance.map.GetTile(tPos.x,tPos.y).occupiedBy == target) {
                    Debug.Log("found target");
                    break;
                }
            }

            return target;
        }

        return null;

    }
}
