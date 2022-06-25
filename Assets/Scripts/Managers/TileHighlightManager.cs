using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileHighlightManager : MonoBehaviour
{
    public static TileHighlightManager instance;

    public Color redHighlight;
    public Color blueHighlight;
    public List<Tile> highlightedTiles = new List<Tile>();
    public List<Tile> prevHighlightedTiles = new List<Tile>();


    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }


    public void ClearPrevHighlights() {
        foreach (Tile t in prevHighlightedTiles) {
            t.SetHighlight(null);
        }
        prevHighlightedTiles.Clear();
    }

    public void ClearHighlights() {
        prevHighlightedTiles = new List<Tile>(highlightedTiles);
        highlightedTiles.Clear();
    }



    public void AddTempHighlight(Tile t, HighlightType type) {
        if (type == HighlightType.none) {
			t.SetHighlight(null);
		} else if (type == HighlightType.red) {
			t.SetHighlight(redHighlight);
            highlightedTiles.Add(t);
		} else if (type == HighlightType.blue) {
			t.SetHighlight(blueHighlight);
            highlightedTiles.Add(t);
		}
    }

    public void AddHighlight(Tile t, HighlightType type) {
        if (type == HighlightType.red) {
			t.SetHighlight(redHighlight);
		} else if (type == HighlightType.blue) {
			t.SetHighlight(blueHighlight);
		}
    }

    public void RemoveHighlight(Tile t) {
        t.SetHighlight(null);
    }

    public void AddOneTurnHighlight(Tile t, UnitController owner, HighlightType type) {
        new TileHighlight(t, owner, type);
    }


    public class TileHighlight {
        public Tile tile;
        public UnitController owner;

        public TileHighlight (Tile t, UnitController owner, HighlightType type) {
            tile = t;
            this.owner = owner;
            owner.OnTurnStart += RemoveHighlight;
            owner.unitStats.OnDie += RemoveHighlight;

            TileHighlightManager.instance.AddHighlight(tile, type);
        }

        void RemoveHighlight() {
            tile.SetHighlight(null);
            owner.OnTurnStart -= RemoveHighlight;
            owner.unitStats.OnDie -= RemoveHighlight;
        }
    }
}

public enum HighlightType {
	none,
	red,
    blue
}