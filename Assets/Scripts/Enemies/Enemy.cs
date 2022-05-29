using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemies/Enemy")]
public class Enemy : ScriptableObject {
    public BaseUnitStats stats;

    public void Controls(EnemyController controller) {
        if (controller.targetUnit == null) {
			controller.targetUnit = controller.vision.FindTarget();
		}

		if (controller.targetUnit != null) {
			int dx = controller.targetUnit.x - controller.x;
			int dz = controller.targetUnit.y - controller.y;

			if (Mathf.Sqrt((dx * dx) + (dz * dz)) < 1.5f) {
                // Attack
				//controller.state = EnemyController.State.Attacking;

				controller.equipmentManager.GetMainWeapon().Attack((UnitController)controller.targetUnit);
			} else {
				List<Vector2Int> targetPath = Game.instance.map.FindPath(new Vector2Int(controller.x, controller.y), new Vector2Int(controller.targetUnit.x, controller.targetUnit.y));

                if (targetPath != null) {
                    // Move to target, position 0 is our own starting position, so skip
                    controller.moveable.BaseMove(targetPath[1].x, targetPath[1].y);
					controller.moveable.isMoving = true;
                } else {
                    //Debug.Log("No Path");
                }
			}
		}
    }
}
