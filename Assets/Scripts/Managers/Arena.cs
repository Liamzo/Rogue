using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : Game
{
	public List<Vector2Int> SpawnPoints = new List<Vector2Int>();
	public int spawnNumber = 10;

    public int[] spawnChance;

	override protected void Start() {
		base.Start();

        SpawnPoints.Add(new Vector2Int(9, 17)); // Top
        SpawnPoints.Add(new Vector2Int(17, 9)); // Right
        SpawnPoints.Add(new Vector2Int(1, 9)); // Left
        SpawnPoints.Add(new Vector2Int(9, 1)); // Bottom

        SpawnPoints.Add(new Vector2Int(5, 17)); // Top
        SpawnPoints.Add(new Vector2Int(13, 17)); // Top

        SpawnPoints.Add(new Vector2Int(17, 12)); // Right
        SpawnPoints.Add(new Vector2Int(17, 6)); // Right

        SpawnPoints.Add(new Vector2Int(1, 12)); // Left
        SpawnPoints.Add(new Vector2Int(1, 6)); // Left

        SpawnPoints.Add(new Vector2Int(5, 1)); // Bottom
        SpawnPoints.Add(new Vector2Int(13, 1)); // Bottom

        SpawnPoints.Add(new Vector2Int(3, 17)); // Top
        SpawnPoints.Add(new Vector2Int(15, 17)); // Top

        SpawnPoints.Add(new Vector2Int(17, 15)); // Right
        SpawnPoints.Add(new Vector2Int(17, 3)); // Right

        SpawnPoints.Add(new Vector2Int(1, 15)); // Left
        SpawnPoints.Add(new Vector2Int(1, 3)); // Left

		SpawnPoints.Add(new Vector2Int(3, 1)); // Bottom
        SpawnPoints.Add(new Vector2Int(15, 1)); // Bottom
	}

	override protected void Update() {
		base.Update();

		if (Input.GetKeyDown(KeyCode.P)) {
			SpawnWave();
		}
	}



	public void SpawnWave() {
        int spawnChanceMax = 0;

        for (int i = 0; i < spawnChance.Length; i++) {
            spawnChanceMax += spawnChance[i];
        }


		for (int i = 0; i < spawnNumber; i++) {
            int rng = Random.Range(0, spawnChanceMax);

            int j;
            for (j = 0; j < spawnChance.Length; j++) {
                rng -= spawnChance[j];

                if (rng < 0) {
                    break;
                }
            }

			map.CreateEnemy(enemyTypes[j], SpawnPoints[i].x, SpawnPoints[i].y);
		}
	}

}