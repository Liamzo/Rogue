using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Flowing Surge", menuName = "Skills/FlowingSurge")]
public class FlowingSurge : Skill
{
    public override bool Use (BaseSkill baseSkill) {
        if (baseSkill.openTargerts.Count == 0) {
			FindTargets(baseSkill);
			if (baseSkill.openTargerts.Count == 0) {
				baseSkill.closedTargerts.Clear();
				return true;	
			}
		}

		// Highlight openTarget tiles
		foreach(Vector2Int pos in baseSkill.openTargerts) {
			baseSkill.game.highlightedTiles.Add(baseSkill.game.map.GetTile(pos.x,pos.y));
		}

		if (Input.GetMouseButtonDown(0)) {
			// if (baseSkill.owner.IsPointerOverGameObject()) {     //Don't take input if mouse is over ui
			//     return;
			// }

			Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2Int clickedPos = baseSkill.game.map.GetXY(worldPosition);

			if (baseSkill.openTargerts.Contains(clickedPos)) {
				Tile tile = baseSkill.game.map.GetTile(clickedPos.x,clickedPos.y);

				if (tile.occupiedBy != null) {
					if (tile.occupiedBy is UnitController) {
						UnitController targetUnit = (UnitController)tile.occupiedBy;

						baseSkill.owner.equipmentManager.GetMainWeapon().Attack(targetUnit);
						baseSkill.closedTargerts.Add(targetUnit);

						// Find move position
						int dx = clickedPos.x - baseSkill.owner.x;
						int dy = clickedPos.y - baseSkill.owner.y;

						baseSkill.owner.BaseMove(targetUnit.x + dx, targetUnit.y + dy);
						baseSkill.owner.unitState = UnitController.UnitState.Moving;

						baseSkill.Reset();
						return false;
					}
				} else {
					// Find target unit
					int dx = (clickedPos.x - baseSkill.owner.x) / 2;
					int dy = (clickedPos.y - baseSkill.owner.y) / 2;

					tile = baseSkill.game.map.GetTile(new Vector2Int(clickedPos.x - dx, clickedPos.y - dy));
					if (tile.occupiedBy == null) {
						Debug.LogWarning("No target found, math gone wrong");
						baseSkill.Reset();
						return true;
					}

					baseSkill.owner.equipmentManager.GetMainWeapon().Attack((UnitController)tile.occupiedBy);
					baseSkill.closedTargerts.Add((UnitController)tile.occupiedBy);

					baseSkill.owner.BaseMove(clickedPos.x, clickedPos.y);
					baseSkill.owner.unitState = UnitController.UnitState.Moving;

					baseSkill.Reset();
					return false;
				}
			}
		}

		return false;
    }

    public override void Activate(BaseSkill baseSkill) {
        base.Activate(baseSkill);
		FindTargets(baseSkill);
    }

	private void FindTargets(BaseSkill baseSkill) {
		// On call, save prev targers in a temp list, and check new ones aren't the same (wont stop loops)
		baseSkill.openTargerts.Clear();

		Vector2Int[] neighbourOffsetArray = new Vector2Int[]{
            new Vector2Int(-1,0),
            new Vector2Int(+1,0),
            new Vector2Int(0,+1),
            new Vector2Int(0,-1),
            new Vector2Int(-1,-1),
            new Vector2Int(-1,+1),
            new Vector2Int(+1,-1),
            new Vector2Int(+1,+1),
        };

		UnitController target;

		foreach (Vector2Int off in neighbourOffsetArray) {
			Vector2Int pos = new Vector2Int(baseSkill.owner.x + off.x,baseSkill.owner.y + off.y);
			Object o;

			if (baseSkill.game.map.IsPositionClear(pos, out o)) {
				continue;

			}

			if (o is UnitController) {
				target = (UnitController) o;
			} else {
				continue;
			}

			if (baseSkill.closedTargerts.Contains(target)) {
				continue;
			}

			// Check behind target
			int nx = off.x + pos.x;
			int ny = off.y + pos.y;
			Vector2Int movePos = new Vector2Int(nx, ny);

			if (baseSkill.game.map.IsPositionClear(movePos)) {
				baseSkill.openTargerts.Add(pos);
				baseSkill.openTargerts.Add(movePos);
			}
		}
	}
}
