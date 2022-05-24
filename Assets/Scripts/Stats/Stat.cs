using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Stat {
    [SerializeField]
    private int baseValue;

    private List<int> modifiers = new List<int>();

    public int GetValue() {
        int finalValue = baseValue;

        modifiers.ForEach(x => finalValue += x);

        return finalValue;
    }

    public void SetBaseValue(int value) {
        baseValue = value;
    }

    public void AddModifier (int modifier) {
        if (modifier != 0) {
            modifiers.Add(modifier);
        }
    }

    public void RemoveModifier (int modifier) {
        if (modifier != 0) {
            modifiers.Remove(modifier);
        }
    }
}

[System.Serializable]
public enum Stats {
    Grit,
    Grace,
    Sight,
    Strength,
    Perception,
    MeleeDamage,
    RangedDamge,
    Armour
}

[System.Serializable]
public struct StatValue {
    public Stats stat;
    public int value;
}
