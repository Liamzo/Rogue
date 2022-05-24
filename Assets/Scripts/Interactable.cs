using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : Object
{
    public virtual bool interact() {
        Debug.Log("Interact");
        return true;
    }
}
