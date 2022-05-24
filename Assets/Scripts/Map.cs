using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;

public class Map
{
    Game game;

	Texture2D mapImgFile;

	// Map Grid
	public Tile[,] map;
	public int width;
	public int height;
    float cellSize;


	public Map(Texture2D mapImgFile, float cellSize) {
        this.game = Game.instance;
		this.mapImgFile = mapImgFile;
        this.cellSize = cellSize;

		GeneratreTiles();
	}

    void GeneratreTiles() {
		Color32[] allPixels = mapImgFile.GetPixels32();

		width = mapImgFile.width;
		height = mapImgFile.height;

		map = new Tile[width, height];

		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				Color32 pixel = allPixels[i + (j * width)];

				Color32 wall = new Color32(0,0,0,255);
				Color32 floor = new Color32(255,255,255,255);

				if (pixel.Equals(floor)) {
					map[i, j] = new Tile(i, j, game.tilePrefabs[0]);
				} else if (pixel.Equals(wall)) {
					map[i, j] = new Tile(i, j, game.tilePrefabs[1], false, false);
				}

                Debug.DrawLine(GetWorldPosition(i,j), GetWorldPosition(i,j+1), Color.white, 10000f);
                Debug.DrawLine(GetWorldPosition(i,j), GetWorldPosition(i+1,j), Color.white, 10000f);
			}
		}
	}

    public void SpawnThings() {
        new BaseDaggers(game.itemGOPrefab, (Daggers)game.itemPrefabs[5], 1, 2).Spawn();
        new BaseEquipment(game.itemGOPrefab, (Equipment)game.itemPrefabs[1], 2, 2).Spawn();
        new BaseEquipment(game.itemGOPrefab, (Equipment)game.itemPrefabs[4], 1, 3).Spawn();
        new BaseWeapon(game.itemGOPrefab, (MeleeWeapon)game.itemPrefabs[2], 2, 1).Spawn();
        new BaseRangedWeapon(game.itemGOPrefab, (RangedWeapon)game.itemPrefabs[3], 3, 1).Spawn();

        // Initial
        CreateEnemy(game.enemyTypes[0], 2, 6);
        CreateEnemy(game.enemyTypes[0], 4, 7);
        CreateEnemy(game.enemyTypes[1], 3, 9);


        // Top Left Room
        CreateEnemy(game.enemyTypes[0], 1, 18);
        CreateEnemy(game.enemyTypes[1], 3, 18);
        CreateEnemy(game.enemyTypes[0], 5, 18);
        CreateEnemy(game.enemyTypes[0], 1, 14);
        CreateEnemy(game.enemyTypes[0], 5, 14);


        // Middle
        CreateEnemy(game.enemyTypes[0], 11, 13);
        CreateEnemy(game.enemyTypes[0], 1, 14);
        CreateEnemy(game.enemyTypes[0], 9, 9);
        CreateEnemy(game.enemyTypes[0], 15, 9);
        CreateEnemy(game.enemyTypes[1], 15, 10);


        // Corridor
        CreateEnemy(game.enemyTypes[1], 9, 18);
        CreateEnemy(game.enemyTypes[0], 13, 16);
    }

    public void CreateEnemy (Enemy enemyType, int x, int y) {
        game.enemyGOPrefab.SetActive(false);

        GameObject enemy = GameObject.Instantiate(game.enemyGOPrefab, new Vector2(x, y), Quaternion.Euler(0, 0, 0));
        EnemyController ec = enemy.GetComponent<EnemyController>();
        ec.enemyType = enemyType;
        ec.x = x;
        ec.y = y;

        UnitStats us = enemy.GetComponent<UnitStats>();
        us.baseUnitStats = enemyType.stats;

        game.units.Add(ec);

        enemy.SetActive(true);
    }


    public Tile GetTile(int x, int y) {
        if (x >= 0 && y >= 0 && x < width && y < height) {
            return map[x,y];
        }

        return null;
    }
    public Tile GetTile(Vector2 worldPosition) {
        Vector2Int pos = GetXY(worldPosition);

        return GetTile(pos.x, pos.y);
    }


    public bool IsWithinMap(Vector2Int position) {
        if (position.x >= 0 && position.x < width && position.y >= 0 && position.y < height) {
            Tile t = GetTile(position.x, position.y);
			if (t.isWalkable) {
				return true;
			}
		}

		return false;
    }

    public bool IsPositionClear(Vector2Int position) {
        if (IsWithinMap(position)) {
            Tile t = GetTile(position.x, position.y);
			if (t.occupiedBy == null) {
				return true;
			}
		}

		return false;
    }
    public bool IsPositionClear(Vector2Int position, out Object blocked) {
        blocked = null;
        if (IsWithinMap(position)) {
            Tile t = GetTile(position.x, position.y);
			if (t.occupiedBy == null) {
				return true;
			}

            blocked = t.occupiedBy;
		}
        
		return false;
    }

    // Util
    private Vector2 GetWorldPosition(int x, int y) {
        return new Vector2(x,y) * cellSize;
    }

    public Vector2Int GetXY(Vector2 worldPosition) {
        int x = Mathf.FloorToInt(worldPosition.x / cellSize);
        int y = Mathf.FloorToInt(worldPosition.y / cellSize);

        return new Vector2Int(x,y);
    }

    public List<Vector2Int> FindPath(Vector2Int startPosition, Vector2Int endPosition) {
        NativeList<int2> path = FindPath(new int2(startPosition.x,startPosition.y), new int2(endPosition.x,endPosition.y));

        List<Vector2Int> vPath = ConvertPath(path);

        path.Dispose();

        if (vPath.Count == 0) {
            vPath = null;
        }

        return vPath;
    }

    #region Pathfinding

    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    private NativeList<int2> FindPath(int2 startPosition, int2 endPosition) {
        NativeArray<PathNode> pathNodeArray = new NativeArray<PathNode>(width * height, Allocator.Temp);

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                PathNode pathNode = new PathNode();
                pathNode.x = x;
                pathNode.y = y;
                pathNode.index = CalculateIndex(x, y);

                pathNode.gCost = int.MaxValue;
                pathNode.hCost = CalculateDistanceCost(new int2(x, y), endPosition);
                pathNode.CalculateFCost();

                if (map[x,y].isWalkable && map[x,y].occupiedBy == null) {
                    pathNode.isWalkable = true;
                } else {
                    pathNode.isWalkable = false;
                }

                //pathNode.isWalkable = map[x,y].isWalkable;

                pathNode.cameFromNodeIndex = -1;

                pathNodeArray[pathNode.index] = pathNode;
            }
        }

        NativeArray<int2> neighbourOffsetArray = new NativeArray<int2>(new int2[] {
            new int2(-1,0),
            new int2(+1,0),
            new int2(0,+1),
            new int2(0,-1),
            new int2(-1,-1),
            new int2(-1,+1),
            new int2(+1,-1),
            new int2(+1,+1),
        }, Allocator.Temp);

        // Assume the end node is walkable, stops breaking when targeting an object
        int endNodeIndex = CalculateIndex(endPosition.x, endPosition.y);
        PathNode endNode = pathNodeArray[endNodeIndex];
        endNode.isWalkable = true;
        pathNodeArray[endNodeIndex] = endNode;

        PathNode startNode = pathNodeArray[CalculateIndex(startPosition.x, startPosition.y)];
        startNode.gCost = 0;
        startNode.CalculateFCost();
        pathNodeArray[startNode.index] = startNode;

        NativeList<int> openList = new NativeList<int>(Allocator.Temp);
        NativeList<int> closedList = new NativeList<int>(Allocator.Temp);

        openList.Add(startNode.index);

        while (openList.Length > 0) {
            int currentNodeIndex = GetLowestCostFNodeIndex(openList, pathNodeArray);
            PathNode currentNode = pathNodeArray[currentNodeIndex];

            if (currentNodeIndex == endNodeIndex) {
                // Reached target
                break;
            }

            // Remove current node from Open List
            for (int i = 0; i < openList.Length; i++) {
                if (openList[i] == currentNodeIndex) {
                    openList.RemoveAtSwapBack(i);
                    break;
                }
            }

            closedList.Add(currentNodeIndex);

            for (int i = 0; i < neighbourOffsetArray.Length; i++) {
                int2 neighbourOffset = neighbourOffsetArray[i];
                int2 neighbourPosition = new int2(currentNode.x + neighbourOffset.x, currentNode.y + neighbourOffset.y);

                if (!IsPositionInsideGrid(neighbourPosition)) {
                    // Not a valid position
                    continue;
                }

                int neighbourNodeIndex = CalculateIndex(neighbourPosition.x, neighbourPosition.y);

                PathNode neighbourNode = pathNodeArray[neighbourNodeIndex];
                if (!neighbourNode.isWalkable) {
                    // Not walkable
                    continue;
                }

                if (closedList.Contains(neighbourNodeIndex)) {
                    // Already searched node
                    continue;
                }

                int2 currentNodePosition = new int2(currentNode.x, currentNode.y);

                int tentativeGCost = currentNode.gCost + CalculateDistanceCost(currentNodePosition, neighbourPosition);
                if (tentativeGCost < neighbourNode.gCost) {
                    neighbourNode.cameFromNodeIndex = currentNodeIndex;
                    neighbourNode.gCost = tentativeGCost;
                    neighbourNode.CalculateFCost();
                    pathNodeArray[neighbourNodeIndex] = neighbourNode;

                    if (!openList.Contains(neighbourNode.index)) {
                        openList.Add(neighbourNode.index);
                    }
                }
            }
        }

        //PathNode 
        endNode = pathNodeArray[endNodeIndex];

        NativeList<int2> path = CalculatePath(pathNodeArray, endNode);

        // foreach (int2 pathPosition in path) {
        //     Debug.Log(pathPosition);
        // }

        //List<Vector2Int> newPath = ConvertPath(path);

        pathNodeArray.Dispose();
        openList.Dispose();
        closedList.Dispose();
        //path.Dispose();

        return path;
    }

    
    private NativeList<int2> CalculatePath(NativeArray<PathNode> pathNodeArray, PathNode endNode) {
        if (endNode.cameFromNodeIndex == -1) {
            // Didn't find path
            return new NativeList<int2>(Allocator.Temp);
        } else {
            // Found path
            NativeList<int2> path = new NativeList<int2>(Allocator.Temp);
            path.Add(new int2(endNode.x, endNode.y));

            PathNode currentNode = endNode;
            while (currentNode.cameFromNodeIndex != -1) {
                PathNode cameFromNode = pathNodeArray[currentNode.cameFromNodeIndex];
                path.Add(new int2(cameFromNode.x, cameFromNode.y));
                currentNode = cameFromNode;
            }

            NativeList<int2> truePath = new NativeList<int2>(Allocator.Temp);
            for (int i = path.Length - 1; i >= 0; i--) {
                truePath.Add(path[i]);
            }

            path.Dispose();

            return truePath;
        }
    }

    private List<Vector2Int> ConvertPath(NativeList<int2> oldPath) {
        List<Vector2Int> path = new List<Vector2Int>();

        foreach (int2 i in oldPath) {
            path.Add(new Vector2Int(i.x,i.y));
        }

        return path;
    }

    private bool IsPositionInsideGrid(int2 gridPosition) {
        return 
            gridPosition.x >= 0 &&
            gridPosition.y >= 0 &&
            gridPosition.x < width &&
            gridPosition.y < height;
    }

    private int CalculateIndex(int x, int y) {
        return x + y * width;
    }

    private int CalculateDistanceCost(int2 aPosition, int2 bPosition) {
        int xDistance = math.abs(aPosition.x - bPosition.x);
        int yDistance = math.abs(aPosition.y - bPosition.y);
        int remaining = math.abs(xDistance - yDistance);
        return MOVE_DIAGONAL_COST * math.min(xDistance, yDistance) + MOVE_STRAIGHT_COST * remaining;
    }

    private int GetLowestCostFNodeIndex(NativeList<int> openList, NativeArray<PathNode> pathNodeArray) {
        PathNode lowestCostPathNode = pathNodeArray[openList[0]];

        for (int i = 1; i < openList.Length; i++) {
            PathNode testPathNode = pathNodeArray[openList[i]];
            if (testPathNode.fCost < lowestCostPathNode.fCost) {
                lowestCostPathNode = testPathNode;
            }
        }

        return lowestCostPathNode.index;
    }

    private struct PathNode {
        public int x;
        public int y;

        public int index;

        public int gCost;
        public int hCost;
        public int fCost;

        public bool isWalkable;

        public int cameFromNodeIndex;

        public void CalculateFCost() {
            fCost = gCost + hCost;
        }
    }

    #endregion


    // FOV
    public bool CheckSight(int x, int y) {
        Tile t = GetTile(x, y);

        if (t == null) {
            return false;
        }

        return t.isViewable;
    }
    
    public void SetLight(int x, int y, float distanceSquared) {
		if (GetTile(x,y) != null) {
            // Make tile visible
			map[x,y].tileSprite.enabled = true;
            map[x,y].tileSprite.color = Color.white;
			map[x,y].visible = true;
            map[x,y].explored = true;

            // If an object is here, make visible
            if (map[x,y].occupiedBy != null) {
                map[x,y].occupiedBy.spriteRenderer.enabled = true;
            }

            // If an item is here, make visible
            foreach(BaseItem item in game.items) {
                if (item.x == x && item.y == y) {
                    item.itemGO.GetComponent<SpriteRenderer>().enabled = true;
                }
            }
		}
	}
	
    public void SetLightOff(int x, int y) {
		if (GetTile(x,y) != null) {
			map[x,y].visible = false;
            map[x,y].tileSprite.color = Color.grey;

            // If an object is here, make invisible
            if (map[x,y].occupiedBy != null) {
                map[x,y].occupiedBy.spriteRenderer.enabled = false;
            }

            // If an item is here, make invisible
            foreach(BaseItem item in game.items) {
                if (item.x == x && item.y == y) {
                    item.itemGO.GetComponent<SpriteRenderer>().enabled = false;
                }
            }
		}
	}
}
