using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dash", menuName = "Skills/Dash")]
public class Dash : Skill
{
    public override CommandResult Use (BaseSkill baseSkill) {    
        // Find and highlight path
        Tile tile = baseSkill.game.map.GetTileUnderMouse();
        Vector2Int targetCoords = new Vector2Int(tile.x, tile.y);

        int xDistance = targetCoords.x - baseSkill.owner.x;
        int yDistance = targetCoords.y - baseSkill.owner.y;
        int remaining = Mathf.Abs(Mathf.Abs(xDistance) - Mathf.Abs(yDistance));
        int min = Mathf.Min(Mathf.Abs(xDistance), Mathf.Abs(yDistance));

        if (remaining + min > baseSkill.owner.unitStats.currentGrace) {
            return new CommandResult(CommandResult.CommandState.Pending, null);
        }

        int max = Mathf.Max(Mathf.Abs(xDistance), Mathf.Abs(yDistance));

        float xStep = xDistance / (float) max;
        float yStep = yDistance / (float) max;

        List<Vector2Int> path = new List<Vector2Int>();

        for (int i = 1; i <= remaining + min; i++) {
            int xPos = Mathf.RoundToInt(xStep * i) + baseSkill.owner.x;
            int yPos = Mathf.RoundToInt(yStep * i) + baseSkill.owner.y;
            Vector2Int tPos = new Vector2Int(xPos,yPos);

            if (!baseSkill.game.map.IsPositionClear(baseSkill.game.map.GetXY(tPos))) {
                break;
            }

            path.Add(tPos);
        }
        
        targetCoords = path[path.Count - 1];        
        foreach(Vector2Int tPos in path) {
            baseSkill.game.highlightedTiles.Add(baseSkill.game.map.GetTile(tPos.x,tPos.y));
        }

        int cost = path.Count;


        if (Input.GetMouseButtonDown(0)) {
            baseSkill.owner.GetComponent<Moveable>().BaseMove(targetCoords.x, targetCoords.y);
			baseSkill.owner.GetComponent<Moveable>().isMoving = true;

            baseSkill.owner.unitStats.AddOrRemoveGrace(-cost);

            return new CommandResult(CommandResult.CommandState.Succeeded, null);
        }
        

        return new CommandResult(CommandResult.CommandState.Pending, null);
    }
}
