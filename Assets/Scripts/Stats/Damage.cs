using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage
{
    public UnitController attacker;
    public int damage;

    public Damage(UnitController att, int dam) {
        attacker = att;
        damage = dam;
    }
}
