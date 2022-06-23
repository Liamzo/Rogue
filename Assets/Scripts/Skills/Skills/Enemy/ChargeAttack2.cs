using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChargeAttack2", menuName = "Skills/Enemy/ChargeAttack2")]
public class ChargeAttack2 : Skill
{
    // Start is called before the first frame update
    public override CommandResult Use(BaseSkill baseSkill) {
        // Remove highlights on all three tiles
        // Damage all 3 targets
        foreach (Vector2Int targetPos in baseSkill.openTargerts) {
            Tile tile = Game.instance.map.GetTile(targetPos.x, targetPos.y);
            TileHighlightManager.instance.RemoveHighlight(tile);

            if (tile.occupiedBy == null) {
                continue;
            }
            if (tile.occupiedBy is UnitController) {
                UnitController target = (UnitController)tile.occupiedBy;
                target.unitStats.TakeDamge(new Damage(baseSkill.owner, baseSkill.owner.unitStats.stats[(int)Stats.Strength].GetValue() + baseSkill.owner.unitStats.stats[(int)Stats.MeleeDamage].GetValue()));

                if (target.unitStats.currentGrit <= 0 && baseSkill.owner is PlayerController) {
                    ((PlayerController) baseSkill.owner).KilledEnemy();

                    if (tile == baseSkill.target) {
                        baseSkill.owner.vision.ChangeTargetUnit(null);
                    }
                }
            }
        }

        Game.instance.TriggerShake();

        return new CommandResult(CommandResult.CommandState.Succeeded, null);
    }
}
