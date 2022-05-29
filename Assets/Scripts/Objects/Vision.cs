using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitController))]
public abstract class Vision : MonoBehaviour
{
    protected UnitController parent;
    protected Game game;

    public abstract UnitController FindTarget();

    protected virtual void Start() {
        parent = GetComponent<UnitController>();
        game = Game.instance;
    }
}
