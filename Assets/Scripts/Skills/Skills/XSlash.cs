using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "X Slash", menuName = "Skills/XSlash")]
public class XSlash : Skill {
    public int bonusDamage;

    public override bool Use (BaseSkill baseSkill) {
        if (Input.GetMouseButtonDown(0)) {
            // if (baseSkill.owner.IsPointerOverGameObject()) {     //Don't take input if mouse is over ui
            //     return;
            // }

            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Tile tile = baseSkill.game.map.GetTile(worldPosition);

            if (tile != null) {
                if (tile.occupiedBy != null) {
                    BaseDaggers weapon = (BaseDaggers) baseSkill.owner.equipmentManager.GetMainWeapon();

                    ((UnitController)tile.occupiedBy).unitStats.TakeDamge(baseSkill.owner.unitStats.stats[(int)Stats.Strength].GetValue() + baseSkill.owner.unitStats.stats[(int)Stats.MeleeDamage].GetValue() + weapon.GetOffHandDamage() + bonusDamage);

                    if (((UnitController)tile.occupiedBy).unitStats.currentGrit <= 0) {
                        Debug.Log("Killed " + ((UnitController)tile.occupiedBy).name);
                        baseSkill.owner.unitStats.AddOrRemoveGrace(1);
                    }

                    return true;
                }
            }
        }

        return false;
    }
}
