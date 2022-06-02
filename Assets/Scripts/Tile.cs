using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public int x;
	public int y;

	public GameObject tilePrefab;
	public MeshRenderer tileSprite;
	public GameObject tile;

	public bool isWalkable;

    public Object occupiedBy;

	// For pathfinding
	public bool visited;
	public float minCost = Mathf.Infinity;
	public float distToTarget;
	public Tile nearestToStart;

    // For FOV
	public bool isViewable;
	public bool visible = false;
	public bool explored = false;

    public Tile(int x, int y, GameObject tilePrefab, bool isWalkable = true, bool isViewable = true) {
		this.x = x;
		this.y = y;
		this.tilePrefab = tilePrefab;
		this.isWalkable = isWalkable;
		this.isViewable = isViewable;

		tile = GameObject.Instantiate(tilePrefab, new Vector3(x+0.5f, y+0.5f, 0), Quaternion.Euler(0, 0, 0));
		tileSprite = tile.GetComponentInChildren<MeshRenderer>();
		tileSprite.enabled = false;
	}

	public void SetHighlight(HighlightType highlight) {
		// if (highlight == HighlightType.none) {
		// 	tileSprite.color = Color.white;
		// } else if (highlight == HighlightType.red) {
		// 	tileSprite.color = Color.red;
		// }
	}
}

public enum HighlightType {
	none,
	red
}