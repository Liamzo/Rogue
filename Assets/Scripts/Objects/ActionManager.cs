using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UnitController))]
public class ActionManager : MonoBehaviour
{
    public UnitController parent;
    public GameObject sprite;
    public bool isAttacking = false;
    public Vector2? aimPos = null;

    // Start is called before the first frame update
    void Start()
    {
        parent = GetComponent<UnitController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isAttacking) {
            Bump();
        }
    }

    public void Bump () {
        if (aimPos == null) {
            Debug.LogError("aimPos is not set");
            isAttacking = false;
            return;
        }

        float step =  10f * Time.deltaTime; // calculate distance to move

        Vector2 newPos = Vector2.MoveTowards(sprite.transform.localPosition, aimPos.Value, step);
        sprite.transform.localPosition = new Vector3(newPos.x, newPos.y, 0f);

        // Check if the position of the cube and sphere are approximately equal.
        if (Vector2.Distance(sprite.transform.localPosition, aimPos.Value) < 0.001f) {
            if (aimPos == new Vector2(0.5f, 0.1f)) {
                sprite.transform.localPosition = new Vector3(0.5f, 0.1f, 0f);
                isAttacking = false;
                aimPos = null;
            } else {
                aimPos = new Vector2(0.5f, 0.1f);
            }
        }
    }




    public void SetAimPos(Vector2Int pos) {
        Vector2 dir = ((Vector2)(pos - new Vector2Int(parent.x, parent.y))).normalized;
        dir = dir / 3;
        aimPos = (Vector2)sprite.transform.localPosition + dir;
    }
}
