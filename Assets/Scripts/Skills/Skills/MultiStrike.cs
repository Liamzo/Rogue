using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Multi Strike", menuName = "Skills/MultiStrike")]
public class MultiStrike : Skill
{
    public int attacks;

    public override bool Use (BaseSkill baseSkill) {
        if (Input.GetMouseButtonDown(0)) {
            // if (baseSkill.owner.IsPointerOverGameObject()) {     //Don't take input if mouse is over ui
            //     return;
            // }

            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Tile tile = baseSkill.game.map.GetTile(worldPosition);

            if (tile != null) {
                if (tile.occupiedBy != null) {
                    for (int i = 0; i < attacks; i++) {
                        baseSkill.owner.equipmentManager.GetMainWeapon().Attack((UnitController)tile.occupiedBy);
                    }
                    return true;
                }
            }
        }
        
        return false;
    }
}
