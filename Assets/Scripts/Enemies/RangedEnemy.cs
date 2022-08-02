using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ranged Enemy", menuName = "Enemies/Ranged Enemy")]
public class RangedEnemy : Enemy 
{
    public override Command Controls(EnemyController controller) {
		controller.vision.currentTarget = controller.vision.FindTarget(FindObjectOfType<PlayerController>());
		
		if (controller.vision.currentTarget != null) {
			// If player is close, run away
			// If player can be shot, aim
			// If player can't be shot, move to clear position

			int dx = controller.vision.currentTarget.x - controller.x;
        	int dy = controller.vision.currentTarget.y - controller.y;

        	int dist = Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy));

			if (dist > 3) {
				List<Vector2Int> path = GetClearPath(controller, new Vector2Int(controller.x, controller.y), new Vector2Int(controller.vision.currentTarget.x,controller.vision.currentTarget.y));
				if (path != null) {
					// Clear path, so Aim
					// Highlight tiles, set queued action, return wait
					if (controller.equipmentManager.GetRangedWeapon() != null) {
						Tile target = Game.instance.map.GetTile(controller.vision.currentTarget.x, controller.vision.currentTarget.y);
						
						controller.queuedCommand = new RangedAttackCommand(controller, target, controller.equipmentManager.GetRangedWeapon());

						foreach (Vector2Int aimPos in path) {
							TileHighlightManager.instance.AddOneTurnHighlight(Game.instance.map.GetTile(aimPos.x,aimPos.y), controller, HighlightType.red);
						}

						return new WaitCommand(controller);
					} else {
						Debug.LogError("No ranged weapon on ranged enemy");
					}
				}		
			}

			// Player close or No clear path, so move
			Tile t = FindClearTilePath(controller, controller.vision.currentTarget); // this is broke yst in case i forget

			List<Vector2Int> targetPath = Game.instance.map.FindPath(new Vector2Int(controller.x, controller.y), new Vector2Int(t.x, t.y));

			if (targetPath != null) {
				// Move to target, position 0 is our own starting position, so skip
				return new MoveCommand(controller, targetPath[1].x, targetPath[1].y);
			} else {
				//Debug.Log("No Path");
			}
			
		}

		return new WaitCommand(controller);
    }

	public Tile FindClearTilePath(UnitController parent, UnitController target) {
		List<Vector2Int> possibleTiles = new List<Vector2Int>();
		Vector2Int targetPos = new Vector2Int(target.x, target.y);

		// Check around parent until suitable tile found
		// At least 3 tiles away from target AND Has clear path
		for (int i = -5; i <= 5; i++) {
			for (int j = -5; j <= 5; j++) {
				int x = parent.x + i;
				int y = parent.y + j;

				if (Game.instance.map.IsPositionClear(new Vector2Int(x,y))) {
					int dx = targetPos.x - x;
					int dy = targetPos.y - y;

					int dist = Mathf.Max(Mathf.Abs(dx), Mathf.Abs(dy));

					if (dist > 3) {
						if (GetClearPath(parent, new Vector2Int(x, y), targetPos) != null) {
							possibleTiles.Add(new Vector2Int(x,y));
						}
					}
				}
			}
		}

		if (possibleTiles.Count == 0) {
			// No possible tiles
			Debug.Log("No possible tile");
			return null;
		}

		Vector2Int bestTile =  possibleTiles[0];
		float bestDist = 999f;

		foreach(Vector2Int t in possibleTiles) {
			int dx = parent.x - t.x;
			int dy = parent.y - t.y;

			float dist = ((dx*dx) + (dy*dy));

			if (dist < bestDist) {
				bestDist = dist;
				bestTile = t;
			}
		}

		return Game.instance.map.GetTile(bestTile.x, bestTile.y);
	}

	public List<Vector2Int> GetClearPath(UnitController parent, Vector2Int start, Vector2Int target) {
		bool found = false;

		int xDistance = target.x - start.x;
        int yDistance = target.y - start.y;
        int max = Mathf.Max(Mathf.Abs(xDistance), Mathf.Abs(yDistance));

        float xStep = xDistance / (float) max;
        float yStep = yDistance / (float) max;

        List<Vector2Int> path = new List<Vector2Int>();

        for (int i = 1; i <= ((RangedWeapon)parent.equipmentManager.GetRangedWeapon().item).range; i++) {
            int xPos = Mathf.RoundToInt(xStep * i) + start.x;
            int yPos = Mathf.RoundToInt(yStep * i) + start.y;

            Vector2Int tPos = new Vector2Int(xPos,yPos);

            if (!Game.instance.map.IsWithinMap(tPos)) {
                break;
            }

            path.Add(tPos);

            if (tPos == target) {
                found = true;
				//break; // Change depending on look
            }
        }

		if (found == true) {
			return path;
		} else {
			return null;
		}
	}
}
