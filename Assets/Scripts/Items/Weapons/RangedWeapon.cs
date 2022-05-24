using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New RangedWeapom", menuName = "Inventory/RangedWeapom")]
public class RangedWeapon : Weapon {
    public int range;

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
                    hit.unitStats.TakeDamge(baseWeapon.owner.unitStats.stats[(int)Stats.Perception].GetValue() + baseWeapon.owner.unitStats.stats[(int)Stats.RangedDamge].GetValue());

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
