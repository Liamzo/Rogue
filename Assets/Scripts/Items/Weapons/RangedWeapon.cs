using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New RangedWeapom", menuName = "Inventory/RangedWeapom")]
public class RangedWeapon : Weapon 
{
    public virtual Command Aim (BaseWeapon baseWeapon) {
        Vector2 worldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int targetCoords = baseWeapon.game.map.GetXY(worldPosition);

        int xDistance = targetCoords.x - baseWeapon.owner.x;
        int yDistance = targetCoords.y - baseWeapon.owner.y;
        int max = Mathf.Max(Mathf.Abs(xDistance), Mathf.Abs(yDistance));

        float xStep = xDistance / (float) max;
        float yStep = yDistance / (float) max;

        List<Vector2Int> path = new List<Vector2Int>();

        for (int i = 1; i <= ((RangedWeapon)baseWeapon.owner.equipmentManager.GetRangedWeapon().item).range; i++) {
            int xPos = Mathf.RoundToInt(xStep * i) + baseWeapon.owner.x;
            int yPos = Mathf.RoundToInt(yStep * i) + baseWeapon.owner.y;

            Vector2Int tPos = new Vector2Int(xPos,yPos);

            if (!baseWeapon.game.map.IsWithinMap(tPos)) {
                break;
            }

            path.Add(tPos);

            if (tPos == targetCoords) {
                break;
            }
        }

        foreach(Vector2Int tPos in path) {
            baseWeapon.game.highlightedTiles.Add(baseWeapon.game.map.GetTile(tPos.x,tPos.y));
        }

        if (Input.GetMouseButtonDown(0)) {
            return new RangedAttackCommand(baseWeapon.owner, targetCoords, (BaseRangedWeapon)baseWeapon);
        }
        return null;
    }

    public void Attack(BaseWeapon baseWeapon, Vector2Int target, out bool killed) {
        killed = false;

        int xDistance = target.x - baseWeapon.owner.x;
        int yDistance = target.y - baseWeapon.owner.y;
        int max = Mathf.Max(Mathf.Abs(xDistance), Mathf.Abs(yDistance));

        float xStep = xDistance / (float) max;
        float yStep = yDistance / (float) max;

        for (int i = 1; i <= range; i++) {
            int xPos = Mathf.RoundToInt(xStep * i) + baseWeapon.owner.x;
			int yPos = Mathf.RoundToInt(yStep * i) + baseWeapon.owner.y;

            //Object blocked;
            if (!baseWeapon.game.map.IsPositionClear(new Vector2Int(xPos, yPos) , out Object blocked)) {
                if (blocked is UnitController) {
                    UnitController hit = (UnitController) blocked;
                    if (hit.unitStats.currentGrit <= 0) {
                        continue;
                    }
                    baseWeapon.owner.ChangeTargetUnit(hit);
                    hit.unitStats.TakeDamge(new Damage(baseWeapon.owner, baseWeapon.owner.unitStats.stats[(int)Stats.Perception].GetValue() + baseWeapon.owner.unitStats.stats[(int)Stats.RangedDamge].GetValue()));

                    if (hit.unitStats.currentGrit <= 0) {
                        Debug.Log("Killed " + hit.name);
                        killed = true;
                        //baseWeapon.owner.unitStats.AddOrRemoveGrace(1);
                    }
                }

                break;
            }
        }
    }

}
