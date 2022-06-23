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
        gameObject.transform.position = new Vector3 (x,y,0);

        game = Game.instance;

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
