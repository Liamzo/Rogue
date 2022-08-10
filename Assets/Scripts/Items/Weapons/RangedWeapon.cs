using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New RangedWeapom", menuName = "Inventory/RangedWeapom")]
public class RangedWeapon : Weapon 
{
    public int ammoCapacity;
    public int ammoCost;

    public virtual Tile Aim (BaseWeapon baseWeapon) {
        Vector2Int targetCoords;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit rayHit;
        if (Physics.Raycast(ray, out rayHit, 1000.0f)){
            targetCoords = baseWeapon.game.map.GetXY(rayHit.collider.transform.position);
        } else {
            return null;
        }

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

            // if (tPos == targetCoords) {
            //     break;
            // }
        }

        foreach(Vector2Int tPos in path) {
            TileHighlightManager.instance.AddTempHighlight(baseWeapon.game.map.GetTile(tPos.x,tPos.y), HighlightType.red);
        }

        if (Input.GetMouseButtonDown(0)) {
            return baseWeapon.game.map.GetTile(path[path.Count-1]);
        }
        return null;
    }

    public void Attack(BaseWeapon baseWeapon, Tile target, out UnitController targetUnit) {
        // MAYBE return bool if target is in range, so the player won't shoot at a target out of range. Would only happen with auto target, is sight > range
        targetUnit = null;

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
                    baseWeapon.owner.vision.ChangeTargetUnit(hit);

                    targetUnit = hit;

                    base.Attack(baseWeapon, hit);
                }

                break;
            }
        }
    }

}
