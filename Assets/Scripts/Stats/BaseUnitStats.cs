using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Stat Block", menuName = "Stats/New Stat Block")]
public class BaseUnitStats : ScriptableObject {
    public string unitName;

    public Sprite sprite;

    public int turnTime;

    public List<StatValue> stats;
}