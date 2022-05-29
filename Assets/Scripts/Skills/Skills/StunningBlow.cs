using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New StunningBlow", menuName = "Skills/StunBlow")]
public class StunningBlow : Skill
{
    public override bool Use (BaseSkill baseSkill) {
        if (Input.GetMouseButtonDown(0)) {
            // if (baseSkill.owner.IsPointerOverGameObject()) {     //Don't take input if mouse is over ui
            //     return;
            // }

            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Tile tile = baseSkill.game.map.GetTile(worldPosition);

            if (tile != null) {
                if (tile.occupiedBy != null) {
                    BaseEffect effect = new StunEffect((UnitController)tile.occupiedBy, 2);

                    return true;
                }
            }
        }

        return false;
    }
}
