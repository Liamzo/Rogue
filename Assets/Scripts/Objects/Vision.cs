using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitController))]
public abstract class Vision : MonoBehaviour
{
    protected UnitController parent;
    protected Game game;

    public UnitController currentTarget;

    public virtual UnitController FindTarget(UnitController tryTarget) {
        UnitController target  = tryTarget;

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

    public virtual void ChangeTargetUnit(UnitController unit) {
        currentTarget = unit;
    }

    protected virtual void Start() {
        parent = GetComponent<UnitController>();
        game = Game.instance;
    }
}
