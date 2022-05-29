using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoSVision : Vision
{
    public override UnitController FindTarget() {
        UnitController target  = FindObjectOfType<PlayerController>();

        if (target == null) {
            return null;
        }

        int dx = target.x - parent.x;
        int dy = target.y - parent.y;

        float dist = Mathf.Sqrt((dx * dx) + (dy * dy));

        if (dist < parent.unitStats.stats[(int) Stats.Sight].GetValue()) {
            return target;
        } else {
            return null;
        }
    }
}
