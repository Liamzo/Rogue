using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game instance;

    public Map map;
	public Texture2D mapImgFile;
	public List<GameObject> tilePrefabs = new List<GameObject>();

    public List<BaseItem> items;
	public List<Item> itemPrefabs = new List<Item>();
    public GameObject itemGOPrefab;

    public PlayerController player;
    public Command queuedCommand;

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

    public Command currentCommand;

    void Awake () {
        instance = this;

        items = new List<BaseItem>();

        map = new Map(mapImgFile,1f);
    }

    // Start is called before the first frame update
    void Start() {
        state = State.TakingTurns;
        currentCommand = null;
        queuedCommand = null;

        map.SpawnThings();

        units = new List<UnitController>(FindObjectsOfType<UnitController>());

        // Should be moved, see at bottom
        initialPosition = Camera.main.transform.localPosition;
    }

    // Update is called once per frame
    void Update() {
        TileHighlightManager.instance.ClearPrevHighlights();

        if (player.turn == false) {
            Command c = player.Controls();
            if (c != null) {
                queuedCommand = c;
            }
        }

        if (state == State.TakingTurns) {
			TakingTurns();
		} else if (state == State.WaitingOnPlayer) {
			WaitingOnPlayer();
		}


        TileHighlightManager.instance.ClearHighlights();


        Shake();
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
                player.TurnStart();
                return;
            } else {
                // Whole thing will break if enemy is waiting on an animation
                // because of the while true loop, time won't advance
                // requires a whole rework, hopefully not 1 per frame

                currentCommand = u.Turn();

                while (true) {
                    CommandResult result = currentCommand.perform();

                    if (result.state == CommandResult.CommandState.Alternative) {
                        currentCommand = result.alternative;
                    } else if (result.state == CommandResult.CommandState.Succeeded) {
                        break;
                    }
                    // TODO: Failed
                }

                u.TurnEnd();
                currentCommand = null;
            }
		}
    }

    void WaitingOnPlayer() {
        if (currentCommand == null) {
            currentCommand = player.Turn();
            if (currentCommand != null) {
                queuedCommand = null;
            }
        }

        if (currentCommand == null) {
            currentCommand = queuedCommand;
            queuedCommand = null;
        }
    
        if (currentCommand != null) {
            CommandResult result = currentCommand.perform();

            if (result.state == CommandResult.CommandState.Alternative) {
                currentCommand = result.alternative;
                return;
            } else if (result.state == CommandResult.CommandState.Succeeded) {
                player.TurnEnd();
                state = State.TakingTurns;
                currentCommand = null;
                return;
            } else if (result.state == CommandResult.CommandState.Failed) {
                currentCommand = null;
                return;
            } else if (result.state == CommandResult.CommandState.Pending) {
                return;
            }
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




    // Should probably be moved somewhere else
    public float shakeDuration = 0f;
    public float shakeMagnitude = 0.5f;
    public float dampingSpeed = 2f;
    Vector3 initialPosition;

    void Shake()
    {
        if (shakeDuration > 0) {
            //Camera.main.transform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude * shakeDuration;
            Vector3 newPos = initialPosition + Random.insideUnitSphere * shakeMagnitude;

            float step =  5f * Time.deltaTime;
            Camera.main.transform.localPosition = Vector3.MoveTowards(Camera.main.transform.localPosition, newPos, step);
            
            shakeDuration -= Time.deltaTime * dampingSpeed;
        } else {
            shakeDuration = 0f;
            Camera.main.transform.localPosition = initialPosition;
        }
    }

    public void TriggerShake() {
        shakeDuration = 0.2f;
    }

}
