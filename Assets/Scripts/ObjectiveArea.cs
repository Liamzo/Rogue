using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveArea : MonoBehaviour
{
    Game game;
    // Start is called before the first frame update
    void Start()
    {
        game = Game.instance;

        //BoxCollider bc = transform.GetComponent<BoxCollider>();

        //Bounds bounds = bc.bounds;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (other.TryGetComponent<PlayerController>(out PlayerController player)) {
            Debug.Log("GAME WON!!!");
            gameObject.SetActive(false);
        }
    }
}
