using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Object : MonoBehaviour
{
    public int x;
    public int y;

    public bool blocking;

    protected Game game;

    public Transform spriteRenderer;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        game = Game.instance;

        gameObject.transform.position = new Vector3 (x*game.map.cellSize,y*game.map.cellSize,0);
     
        spriteRenderer = transform.Find("Sprite");

        if (blocking == true) {
            game.map.map[x,y].occupiedBy = this;
        }

        if (game.map.map[x,y].visible == false) {
            spriteRenderer.gameObject.SetActive(false);
        } else if (game.map.map[x,y].visible == true) {
            spriteRenderer.gameObject.SetActive(true);
        }
    }
}
