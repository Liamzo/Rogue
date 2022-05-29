using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEffect
{
	protected UnitController owner;
	
	public abstract void OnEnd();
}
