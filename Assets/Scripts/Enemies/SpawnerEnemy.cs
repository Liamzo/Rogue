using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Spawner Enemy", menuName = "Enemies/Spawner Enemy")]
public class SpawnerEnemy : Enemy
{
    public int spawnNumber;
    public int spawnRange;
    public Enemy[] spawnTypes;

    public override Command Controls(EnemyController controller) {
        Map map = Game.instance.map;
        // Get free tiles
        for (int i = -spawnRange; i <= spawnRange; i++) {
            for (int j = -spawnRange; j <= spawnRange; j++) {
                // If position clear, spawn  an enemy
                Vector2Int pos = new Vector2Int(controller.x + i, controller.y + j);

                if (map.IsPositionClear(pos)) {
                    map.CreateEnemy(spawnTypes[0], pos.x, pos.y);
                }
            }
        }

        return new WaitCommand(controller);
    }
}
