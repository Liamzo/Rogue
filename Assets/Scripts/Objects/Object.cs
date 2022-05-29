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

        if (blocking == true) {
            game.map.map[x,y].occupiedBy = this;
        }

        if (game.map.map[x,y].visible == false) {
            spriteRenderer.enabled = false;
        } else if (game.map.map[x,y].visible == true) {
            spriteRenderer.enabled = true;
        }
    }
}
