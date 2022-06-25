using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChargeAttack1", menuName = "Skills/Enemy/ChargeAttack1")]
public class ChargeAttack : Skill
{
    public ChargeAttack2 part2;

    public override CommandResult Use (BaseSkill baseSkill) {
        BaseSkill skill = new BaseSkill(part2);
        skill.owner = baseSkill.owner;
        skill.target = baseSkill.target;

        skill.openTargerts.Add(baseSkill.target.GetPos());
        TileHighlightManager.instance.AddOneTurnHighlight(baseSkill.target, baseSkill.owner, HighlightType.red);

        // Get diff to target
        Vector2Int diff = new Vector2Int(baseSkill.target.x, baseSkill.target.y) - new Vector2Int(baseSkill.owner.x, baseSkill.owner.y);

        // Add 2 adjacent targets in perpendicular line
        List<Vector2Int> possibleTargets = new List<Vector2Int>();

        if (Mathf.Abs(diff.x) == 1 && Mathf.Abs(diff.y) == 1) {
            // Diagonal
            possibleTargets.Add(new Vector2Int(0, diff.y));
            possibleTargets.Add(new Vector2Int(diff.x, 0));
        } else {
            // Straight
            if (diff.x == 0) {
                // Vertical
                possibleTargets.Add(new Vector2Int(-1, diff.y));
                possibleTargets.Add(new Vector2Int(1, diff.y));
            } else {
                // Horizontal
                possibleTargets.Add(new Vector2Int(diff.x, -1));
                possibleTargets.Add(new Vector2Int(diff.x, 1));
            }
        }

        // Highlight all three tiles
        foreach (Vector2Int possibleTarget in possibleTargets) {
            Vector2Int target = new Vector2Int(baseSkill.owner.x, baseSkill.owner.y) + possibleTarget;
            if (Game.instance.map.IsWithinMap(target)) {
                Tile t = Game.instance.map.GetTile(target.x, target.y);
                TileHighlightManager.instance.AddOneTurnHighlight(t, baseSkill.owner, HighlightType.red);
                skill.openTargerts.Add(target);
            }
        }

        baseSkill.owner.queuedCommand = new SkillCommand(baseSkill.owner, skill);
        return new CommandResult(CommandResult.CommandState.Succeeded, null);
    }
}
