using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Swipe", menuName = "Skills/Sword/Swipe")]
public class Swipe : Skill
{
    public override CommandResult Use (BaseSkill baseSkill) {
        if (Input.GetMouseButtonDown(0)) {
            // if (baseSkill.owner.IsPointerOverGameObject()) {     //Don't take input if mouse is over ui
            //     return;
            // }

            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Tile tile = baseSkill.game.map.GetTile(worldPosition);

            if (tile != null) {
                int xDistance = tile.x - baseSkill.owner.x;
                int yDistance = tile.y - baseSkill.owner.y;
                int dist = Mathf.Max(Mathf.Abs(xDistance), Mathf.Abs(yDistance));
                
                if (tile.occupiedBy != null && dist == baseSkill.owner.equipmentManager.GetMainWeapon().item.range) {
                    // Check the movement space is free
                    int moveX = xDistance * -1;
                    int moveY = yDistance * -1;

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
