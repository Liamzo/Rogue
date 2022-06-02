using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "X Slash", menuName = "Skills/XSlash")]
public class XSlash : Skill {
    public int bonusDamage;

    public override CommandResult Use (BaseSkill baseSkill) {
        if (Input.GetMouseButtonDown(0)) {
            // if (baseSkill.owner.IsPointerOverGameObject()) {     //Don't take input if mouse is over ui
            //     return;
            // }

            Tile tile = baseSkill.game.map.GetTileUnderMouse();

            if (tile != null) {
                if (tile.occupiedBy != null) {
                    BaseDaggers weapon = (BaseDaggers) baseSkill.owner.equipmentManager.GetMainWeapon();

                    ((UnitController)tile.occupiedBy).unitStats.TakeDamge(new Damage(baseSkill.owner, baseSkill.owner.unitStats.stats[(int)Stats.Strength].GetValue() + baseSkill.owner.unitStats.stats[(int)Stats.MeleeDamage].GetValue() + weapon.GetOffHandDamage() + bonusDamage));

                    if (((UnitController)tile.occupiedBy).unitStats.currentGrit <= 0) {
                        Debug.Log("Killed " + ((UnitController)tile.occupiedBy).name);
                        baseSkill.owner.unitStats.AddOrRemoveGrace(1);
                    }

                    return new CommandResult(CommandResult.CommandState.Succeeded, null);
                }
            }
        }

        return new CommandResult(CommandResult.CommandState.Pending, null);
    }
}
