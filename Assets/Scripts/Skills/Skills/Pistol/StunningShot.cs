using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New StunningShot", menuName = "Skills/Pistol/StunBlow")]
public class StunningShot : Skill
{
    public int duration;

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

                if (tile.occupiedBy != null && dist <= baseSkill.owner.equipmentManager.GetRangedWeapon().item.range) {
                    baseSkill.owner.equipmentManager.GetRangedWeapon().Attack(new Vector2Int(tile.x, tile.y));
                    BaseEffect effect = new StunEffect((UnitController)tile.occupiedBy, duration);

                    return new CommandResult(CommandResult.CommandState.Succeeded, null);
                }
            }
        }

        return new CommandResult(CommandResult.CommandState.Pending, null);
    }
}
