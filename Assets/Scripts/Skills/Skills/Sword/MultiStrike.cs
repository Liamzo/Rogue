using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Multi Strike", menuName = "Skills/Sword/MultiStrike")]
public class MultiStrike : Skill
{
    public int attacks;

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

            if (dist <= baseSkill.owner.equipmentManager.GetMeleeWeapon().item.range) {
                for (int i = 0; i < attacks; i++) {
                    baseSkill.owner.equipmentManager.GetMeleeWeapon().Attack(target);
                }
                return new CommandResult(CommandResult.CommandState.Succeeded, null);
            }
        }
        
        
        if (baseSkill.target != null) {
            // Failed with given target, so fail rather than check for input
            return new CommandResult(CommandResult.CommandState.Failed, null);
        }
        return new CommandResult(CommandResult.CommandState.Pending, null);
    }
}
