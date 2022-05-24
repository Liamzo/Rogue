using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSkill {
    public Game game;

	public UnitController owner;
	public UnitController targetUnit;
	public Vector2Int targetCoords;
	public float xStep;
    public float yStep;

	public Skill skill;
	public KeyCode hotKey;

	public float clock;

	// For Flowing Strike, need a better way to structure this
	public List<Vector2Int> openTargerts;
	public List<UnitController> closedTargerts;

    public BaseSkill (Skill skill) {
		this.game = Game.instance;
		this.skill = skill;
		this.clock = 0;

		openTargerts = new List<Vector2Int>();
		closedTargerts = new List<UnitController>();
	}

	public void OnUnlock () {
		skill.OnUnlock();
	}

	public bool Use () {
        bool done = skill.Use(this);
		if (done) {
            Reset();
        }
        return done;
    }

	public virtual void Reset() {
		targetUnit = null;
		targetCoords = Vector2Int.zero;
		xStep = 0;
		yStep = 0;

		clock = 0;
		openTargerts.Clear();
    }

    public void Effects () {
        skill.Effects();
    }

	public bool CanBeActivated() {
		if (owner.unitStats.currentGrace >= skill.graceCost && skill.CanActivate(this)) {
			return true;
		}

		return false;
	}
}
