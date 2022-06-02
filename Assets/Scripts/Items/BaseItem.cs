using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseItem {
	public Game game;
	public UnitController owner;
	public Item item;
	public int x;
	public int y;

	public GameObject itemGOPrefab;
	public GameObject itemGO;

	public BaseItem (GameObject itemGOPrefab, Item item, int x, int y) {
		this.game = Game.instance;
		this.item = item;
		this.x = x;
		this.y = y;

		this.itemGOPrefab = itemGOPrefab;
		this.itemGO = null;
	}

	public virtual void PickUp (UnitController pickedUpBy) {
		if (itemGO != null) {
			Debug.Log("Picking up " + item.name);
			bool wasPickedUp = Inventory.instance.Add(this);

			if (wasPickedUp) {
				Debug.Log("Was picked up " + item.name);
				game.items.Remove(this);
				GameObject.Destroy(itemGO);
				itemGO = null;

				owner = pickedUpBy;
			}
		}
	}

	public void Drop () {
		Debug.Log("Dropping " + item.name);
		x = game.player.x;
		y = game.player.y;
		Spawn();
		//Game.instance.items.Add(this);
		RemoveFromInventory();
		owner = null;
	}

	public void Spawn () {
		itemGO = GameObject.Instantiate(itemGOPrefab, new Vector3(x+0.5f, y+0.5f, -0.1f), Quaternion.Euler(0, 0, 0));
		SpriteRenderer sr = itemGO.GetComponent<SpriteRenderer>();
		sr.sprite = item.icon;

		if (game.map.map[this.x,this.y].explored == false) {
			sr.enabled = false;
		}

		game.items.Add(this);
	}


	public virtual void Use () {
		item.Use();
	}

	public void RemoveFromInventory () {
        Inventory.instance.Remove(this);
    }
}