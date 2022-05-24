using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStats : MonoBehaviour {
    public BaseUnitStats baseUnitStats;

    public string unitName;

    public int currentGrit {get; private set;}

    public int currentGrace {get; set;}

    public Stat[] stats;

    public event System.Action OnUIChange;

    void Awake() {
        unitName = baseUnitStats.unitName;

        // Set Sprite
        gameObject.transform.Find("Sprite").GetComponent<SpriteRenderer>().sprite = baseUnitStats.sprite;

        stats = new Stat[System.Enum.GetNames(typeof(Stats)).Length];

        for (int i = 0; i < stats.Length; i++) {
            stats[i] = new Stat();
        }

        // Set stats using BaseUnitStats
        foreach (StatValue sv in baseUnitStats.stats) {
            int slot = (int) sv.stat;
            stats[slot].SetBaseValue(sv.value);
        }

        currentGrit = stats[(int)Stats.Grit].GetValue();
        currentGrace = stats[(int)Stats.Grace].GetValue();
    }

    public void TakeDamge (int damage) {
        damage -=  stats[(int)Stats.Armour].GetValue();
        damage = Mathf.Clamp(damage, 0, int.MaxValue);

        if (currentGrace > 0) {
            AddOrRemoveGrace(-damage);
            Debug.Log(transform.name + " parries the blow");
            return;
        }

        currentGrit -= damage;
        Debug.Log(transform.name + " takes " + damage + " damage");

        if (OnUIChange != null) {
            OnUIChange();
        }

        if (currentGrit <= 0) {
            Die();
        }
    }

    public void TakeTrueDamage (int damage) {
        currentGrit -= damage;
        Debug.Log(transform.name + " takes " + damage + " damage");

        if (OnUIChange != null) {
            OnUIChange();
        }


        if (currentGrit <= 0) {
            Die();
        }
    }

    public void AddOrRemoveGrace(int amount) {
        currentGrace += amount;

        currentGrace = Mathf.Clamp(currentGrace, 0,  stats[(int)Stats.Grace].GetValue() * 3);

        if (OnUIChange != null) {
            OnUIChange();
        }

    }

    public virtual void Die() {
        // This should be overwritten
        Debug.Log(transform.name + " died");
        Game.instance.units.Remove(GetComponent<UnitController>());
        Destroy(this.gameObject);
    }
}
