using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Object))]
public class Moveable : MonoBehaviour
{
    public Object parent;
    public bool isMoving = false;

    // Start is called before the first frame update
    void Start()
    {
        parent = GetComponent<Object>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving) {
            Move(parent.x, parent.y);
        }
    }

    public void Move(int x, int y) {
		float step =  10f * Time.deltaTime; // calculate distance to move
        transform.position = Vector2.MoveTowards(transform.position, new Vector2(x*Game.instance.map.cellSize,y*Game.instance.map.cellSize), step);

        // Check if the position of the cube and sphere are approximately equal.
        if (Vector2.Distance(transform.position, new Vector2(x*Game.instance.map.cellSize,y*Game.instance.map.cellSize)) < 0.001f) {
            // Swap the position of the unit
			isMoving = false;
            AudioManager.instance.StopSound(gameObject);
			gameObject.transform.position = new Vector3 (x*Game.instance.map.cellSize,y*Game.instance.map.cellSize,0);

        	if (Game.instance.map.map[x,y].visible == false) {
				parent.spriteRenderer.gameObject.SetActive(false);
			} else if (Game.instance.map.map[x,y].visible == true) {
				parent.spriteRenderer.gameObject.SetActive(true);
			}
        }
	}

    public void BaseMove(int x, int y) {
        if (parent.blocking == true) {
            Game.instance.map.map[parent.x,parent.y].occupiedBy = null;
            Game.instance.map.map[x,y].occupiedBy = parent;
        }

        parent.x = x;
        parent.y = y;

        isMoving = true;
        AudioManager.instance.PlaySound(gameObject);
    }
}
