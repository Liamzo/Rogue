using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New StunningShot", menuName = "Skills/Pistol/StunBlow")]
public class StunningShot : Skill
{
    public int duration;

    public override CommandResult Use (BaseSkill baseSkill) {
        Tile target = null;
        if (baseSkill.target != null) {
            target = baseSkill.target;
        }

        if (Input.GetMouseButtonDown(0) && target == null) {
            // if (baseSkill.owner.IsPointerOverGameObject()) {     //Don't take input if mouse is over ui
            //     return;
            // }

            target = baseSkill.game.map.GetTileUnderMouse();

        }

        if (target != null) {
            int xDistance = target.x - baseSkill.owner.x;
            int yDistance = target.y - baseSkill.owner.y;
            int dist = Mathf.Max(Mathf.Abs(xDistance), Mathf.Abs(yDistance));

            if (target.occupiedBy != null && dist <= baseSkill.owner.equipmentManager.GetRangedWeapon().item.range) {
                baseSkill.owner.equipmentManager.GetRangedWeapon().Attack(target);
                BaseEffect effect = new StunEffect((UnitController)target.occupiedBy, duration);

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
