using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Spray", menuName = "Skills/Spray")]
public class Spray : Skill
{
	public int attacks;

    // public override bool Use (BaseSkill baseSkill) {
    //     if (baseSkill.targetUnit == null && baseSkill.clock == 0) {
    //         if (Input.GetMouseButtonDown(0)) {
    //             // if (baseSkill.owner.IsPointerOverGameObject()) {     //Don't take input if mouse is over ui
	// 			//     return;
	// 		    // }

    //             Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //             Tile tile = baseSkill.game.map.GetTile(worldPosition);

    //             if (tile != null) {
    //                 baseSkill.targetUnit = (UnitController)tile.occupiedBy;
    //                 baseSkill.owner.attackWeapon = baseSkill.owner.equipmentManager.GetMainWeapon();
    //                 baseSkill.owner.attackWeapon.target = baseSkill.targetUnit;
    //             }
    //         }
    //         return false;
    //     }

    //     if (baseSkill.targetUnit != null && baseSkill.clock < attacks) {
    //         if (baseSkill.owner.attackWeapon.Attack()) {
    //             baseSkill.clock += 1;
    //         }
    //         return false;
    //     }

    //     if (baseSkill.targetUnit == null || baseSkill.clock >= attacks) {
    //         // Done all attacks
    //         baseSkill.clock = 0;
    //         baseSkill.targetUnit = null;
    //         return true;
    //     }
        
    //     return false;
    // }
    
}
