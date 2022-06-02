using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Multi Strike", menuName = "Skills/Sword/MultiStrike")]
public class MultiStrike : Skill
{
    public int attacks;

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

                if (tile.occupiedBy != null && dist <= baseSkill.owner.equipmentManager.GetMainWeapon().item.range) {
                    for (int i = 0; i < attacks; i++) {
                        baseSkill.owner.equipmentManager.GetMainWeapon().Attack((UnitController)tile.occupiedBy);
                    }
                    return new CommandResult(CommandResult.CommandState.Succeeded, null);
                }
            }
        }
        return new CommandResult(CommandResult.CommandState.Pending, null);
    }
}
