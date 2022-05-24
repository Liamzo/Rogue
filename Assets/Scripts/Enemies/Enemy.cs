using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemies/Enemy")]
public class Enemy : ScriptableObject {
    public BaseUnitStats stats;

    public void Controls(EnemyController controller) {
        if (controller.targetObject == null) {
			controller.FindTarget();
		}

		if (controller.targetObject != null) {
			int dx = controller.targetObject.x - controller.x;
			int dz = controller.targetObject.y - controller.y;

			if (Mathf.Sqrt((dx * dx) + (dz * dz)) < 1.5f) {
                // Attack
				//controller.state = EnemyController.State.Attacking;

				controller.equipmentManager.GetMainWeapon().Attack((UnitController)controller.targetObject);
			} else {
				List<Vector2Int> targetPath = controller.FindPathToTarget();

                if (targetPath != null) {
                    // Move to target, position 0 is our own starting position, so skip
                    controller.unitState = EnemyController.UnitState.Moving;

                    controller.BaseMove(targetPath[1].x, targetPath[1].y);

                } else {
                    //Debug.Log("No Path");
                }
			}
		}
    }
}
