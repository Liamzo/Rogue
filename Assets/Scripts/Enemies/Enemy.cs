using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemies/Enemy")]
public class Enemy : ScriptableObject {
    public BaseUnitStats stats;

    public Command Controls(EnemyController controller) {
        if (controller.targetUnit == null) {
			controller.targetUnit = controller.vision.FindTarget();
		}

		if (controller.targetUnit != null) {
			List<Vector2Int> targetPath = Game.instance.map.FindPath(new Vector2Int(controller.x, controller.y), new Vector2Int(controller.targetUnit.x, controller.targetUnit.y));

			if (targetPath != null) {
				// Move to target, position 0 is our own starting position, so skip
				return new MoveCommand(controller, targetPath[1].x, targetPath[1].y);
			} else {
				//Debug.Log("No Path");
			}
		}

		return new WaitCommand(controller);
    }
}
