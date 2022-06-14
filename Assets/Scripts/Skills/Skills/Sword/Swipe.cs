using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Swipe", menuName = "Skills/Sword/Swipe")]
public class Swipe : Skill
{
    public override CommandResult Use (BaseSkill baseSkill) {
        UnitController target = null;
        if (baseSkill.target != null) {
            target = (UnitController)baseSkill.target.occupiedBy;
        }

        if (Input.GetMouseButtonDown(0) && target == null) {
            // if (baseSkill.owner.IsPointerOverGameObject()) {     //Don't take input if mouse is over ui
            //     return;
            // }

            Tile tile = baseSkill.game.map.GetTileUnderMouse();
            if (tile.occupiedBy != null) {
                if (tile.occupiedBy is UnitController) {
                    target = (UnitController)tile.occupiedBy;
                }
            }
        }

        if (target != null) {
            int xDistance = target.x - baseSkill.owner.x;
            int yDistance = target.y - baseSkill.owner.y;
            int dist = Mathf.Max(Mathf.Abs(xDistance), Mathf.Abs(yDistance));
            
            if (dist == baseSkill.owner.equipmentManager.GetMainWeapon().item.range) {
                // Check the movement space is free
                int moveX = xDistance * -1;
                int moveY = yDistance * -1;

                if (Game.instance.map.IsPositionClear(new Vector2Int(baseSkill.owner.x + moveX, baseSkill.owner.y + moveY))) {
                    baseSkill.owner.GetComponent<Moveable>().BaseMove(baseSkill.owner.x + moveX, baseSkill.owner.y + moveY);

                    baseSkill.owner.equipmentManager.GetMainWeapon().Attack(target);
                
                    return new CommandResult(CommandResult.CommandState.Succeeded, null);
                }
            }
        }
        
        
        if (baseSkill.target != null) {
            // Failed with given target, so fail rather than check for input
            return new CommandResult(CommandResult.CommandState.Failed, null);
        }
        return new CommandResult(CommandResult.CommandState.Pending, null);
    }
}
