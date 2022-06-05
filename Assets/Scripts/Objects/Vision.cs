using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitController))]
public abstract class Vision : MonoBehaviour
{
    protected UnitController parent;
    protected Game game;

    public UnitController currentTarget;

    public abstract UnitController FindTarget();

    public virtual void ChangeTargetUnit(UnitController unit) {
        currentTarget = unit;
    }

    protected virtual void Start() {
        parent = GetComponent<UnitController>();
        game = Game.instance;
    }
}
