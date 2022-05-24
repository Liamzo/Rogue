using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Object : MonoBehaviour
{
    public int x;
    public int y;

    public bool blocking;

    protected Game game;

    public SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        gameObject.transform.position = new Vector3 (x,y,0);

        game = Game.instance;

        spriteRenderer = transform.Find("Sprite").GetComponent<SpriteRenderer>();

        Move(x,y);

        if (blocking == true) {
            game.map.map[x,y].occupiedBy = this;
        }
    }

    public virtual void Move(int x, int y) {
        if (blocking == true) {
            game.map.map[this.x,this.y].occupiedBy = null;
            game.map.map[x,y].occupiedBy = this;
        }

        this.x = x;
        this.y = y;

        gameObject.transform.position = new Vector3 (x,y,0);

        if (game.map.map[x,y].visible == false) {
            spriteRenderer.enabled = false;
        } else if (game.map.map[x,y].visible == true) {
            spriteRenderer.enabled = true;
        }
    }

    public virtual void BaseMove(int x, int y) {
        if (blocking == true) {
            game.map.map[this.x,this.y].occupiedBy = null;
            game.map.map[x,y].occupiedBy = this;
        }

        this.x = x;
        this.y = y;
    }
}
