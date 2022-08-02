using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Big Enemy", menuName = "Enemies/Big Enemy")]
public class BigEnemy : Enemy {
    public override Command Controls(EnemyController controller) {
		controller.vision.currentTarget = controller.vision.FindTarget(FindObjectOfType<PlayerController>());
		
		if (controller.vision.currentTarget != null) {
			List<Vector2Int> targetPath = Game.instance.map.FindPath(new Vector2Int(controller.x, controller.y), new Vector2Int(controller.vision.currentTarget.x, controller.vision.currentTarget.y));

			if (targetPath != null) {
				// Move to target, position 0 is our own starting position, so skip
				Vector2Int target = new Vector2Int(targetPath[1].x, targetPath[1].y);

				// If in melee range, and can use our skill, then do so
				Vector2Int diff = new Vector2Int(controller.vision.currentTarget.x, controller.vision.currentTarget.y) - new Vector2Int(controller.x, controller.y);
				if (Mathf.Abs(diff.x) <= 1 && Mathf.Abs(diff.y) <= 1) {
					BaseSkill skill = controller.unitSkills.GetSkill(0);

					if (skill != null) {
						skill.target = Game.instance.map.GetTile(controller.vision.currentTarget.x, controller.vision.currentTarget.y);
						return new SkillCommand(controller, skill);
					}
				}

				return new MoveCommand(controller, target.x, target.y);
			} else {
				//Debug.Log("No Path");
			}
		}

		return new WaitCommand(controller);
    }
}
