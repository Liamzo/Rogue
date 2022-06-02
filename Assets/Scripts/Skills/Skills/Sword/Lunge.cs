using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Lunge", menuName = "Skills/Sword/Lunge")]
public class Lunge : Skill
{
    public override CommandResult Use (BaseSkill baseSkill) {
        if (Input.GetMouseButtonDown(0)) {
            // if (baseSkill.owner.IsPointerOverGameObject()) {     //Don't take input if mouse is over ui
            //     return;
            // }

            Tile tile = baseSkill.game.map.GetTileUnderMouse();

            if (tile != null) {
                int xDistance = tile.x - baseSkill.owner.x;
                int yDistance = tile.y - baseSkill.owner.y;
                int dist = Mathf.Max(Mathf.Abs(xDistance), Mathf.Abs(yDistance));

                if (tile.occupiedBy != null && dist == 2) {
                    // Check the movement space is free
                    int moveX = xDistance > 0 ? (int)Mathf.Ceil(xDistance / 2.0f) : (int)Mathf.Floor(xDistance / 2.0f);
                    int moveY = yDistance > 0 ? (int)Mathf.Ceil(yDistance / 2.0f) : (int)Mathf.Floor(yDistance / 2.0f);

                    if (Game.instance.map.IsPositionClear(new Vector2Int(baseSkill.owner.x + moveX, baseSkill.owner.y + moveY))) {
                        baseSkill.owner.GetComponent<Moveable>().BaseMove(baseSkill.owner.x + moveX, baseSkill.owner.y + moveY);
						baseSkill.owner.GetComponent<Moveable>().isMoving = true;

                        baseSkill.owner.equipmentManager.GetMainWeapon().Attack((UnitController)tile.occupiedBy);

                        return new CommandResult(CommandResult.CommandState.Succeeded, null);
                    }
                }
            }
        }

        return new CommandResult(CommandResult.CommandState.Pending, null);
    }
}
