using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Console : Interactable
{
    public GameObject objectiveArea;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        objectiveArea.SetActive(false);
    }

    public override bool interact() {
        base.interact();
        objectiveArea.SetActive(true);
        return true;
    }
}
