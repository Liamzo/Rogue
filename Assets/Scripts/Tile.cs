using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile
{
    public int x;
	public int y;

	public GameObject tilePrefab;
	public MeshRenderer tileSprite;
	public Color baseColour;
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

	public bool highlighted = false;

    public Tile(int x, int y, GameObject tilePrefab, bool isWalkable = true, bool isViewable = true) {
		this.x = x;
		this.y = y;
		this.tilePrefab = tilePrefab;
		this.isWalkable = isWalkable;
		this.isViewable = isViewable;

		tile = GameObject.Instantiate(tilePrefab, new Vector3(x+0.5f, y+0.5f, 0), Quaternion.Euler(0, 0, 0));
		tileSprite = tile.GetComponentInChildren<MeshRenderer>();
		baseColour = tileSprite.material.color;
		tileSprite.enabled = false;
	}

	public void SetHighlight(Color? highlight) {
		if (highlight == null) {
			if (visible == true) {
				tileSprite.material.color = baseColour;
			} else {
				tileSprite.material.color = new Color(0.18f, 0.18f, 0.18f);
			}
			highlighted = false;
		} else {
			tileSprite.material.color = highlight.Value;
			highlighted = true;
		}
	}

	public Vector2Int GetPos() {
		return new Vector2Int(x,y);
	}
}