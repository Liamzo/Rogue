using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stat Block", menuName = "Stats/New Stat Block")]
public class BaseUnitStats : ScriptableObject {
    // public int baseGrit;
    // public int baseGrace;

    // public int baseDamage;
    // public int baseArmour;

    public string unitName;

    public Sprite sprite;

    public List<StatValue> stats;
}