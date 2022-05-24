using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game instance;

    public Map map;
	public Texture2D mapImgFile;
	public List<GameObject> tilePrefabs = new List<GameObject>();
    public Material baseTileMaterial;
    public Material redHighlight;
    public List<Tile> highlightedTiles = new List<Tile>();
    public List<Tile> prevHighlightedTiles = new List<Tile>();

    public List<BaseItem> items;
	public List<Item> itemPrefabs = new List<Item>();
    public GameObject itemGOPrefab;

    public PlayerController player;
    public List<UnitController> units;

    public List<Enemy> enemyTypes;
    public List<BaseUnitStats> unitStats;
    public GameObject enemyGOPrefab;

    public Skill[] allSkills;

    public enum State {
		TakingTurns,
        WaitingOnPlayer
	};
	State state;

    void Awake () {
        instance = this;

        items = new List<BaseItem>();

        map = new Map(mapImgFile,1f);

        map.SpawnThings();

        units = new List<UnitController>(FindObjectsOfType<UnitController>());
    }

    // Start is called before the first frame update
    void Start() {
        state = State.TakingTurns;
    }

    // Update is called once per frame
    void Update() {
        foreach (Tile t in prevHighlightedTiles) {
            t.SetHighlight(HighlightType.none);
        }
        prevHighlightedTiles.Clear();

        if (state == State.TakingTurns) {
			TakingTurns();
		} else if (state == State.WaitingOnPlayer) {
			WaitingOnPlayer();
		}

        foreach (Tile t in highlightedTiles) {
            t.SetHighlight(HighlightType.red);
        }

        prevHighlightedTiles = new List<Tile>(highlightedTiles);
        highlightedTiles.Clear();
    }

    void TakingTurns() {
        // Find the next unit in turn order
        if (units.Count == 0) {
            return;
        }

		UnitController u = null;
        while (u != player) {
			// Get next unit to act
			u = units[0]; // Unit whose turn it is
			foreach (UnitController unit in units) {
				if (unit.turnTimer < u.turnTimer) {
					u = unit;
				} else if (unit.turnTimer == u.turnTimer) {
					if (unit.turnTime < u.turnTime) {
						u = unit;
					}
				}
			}

			// Decrease other units turn times
			foreach (UnitController unit in units) {
				if (!unit.Equals(u)) {
					unit.turnTimer -= u.turnTimer;
				}
			}

			if (u == player) {
                state = State.WaitingOnPlayer;
            } else {
                u.Turn();
            }
		}
    }

    void WaitingOnPlayer() {
        if (player.Turn()) {
            state = State.TakingTurns;
		}
    }






    public BaseItem GetItem(int x, int y) {
        BaseItem item = null;

        foreach (BaseItem i in items) {
            if (i.x == x && i.y == y) {
                item = i;
                break;
            }
        }

        return item;
    }
}
