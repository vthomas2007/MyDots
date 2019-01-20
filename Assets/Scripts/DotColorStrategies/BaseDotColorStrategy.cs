using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseDotColorStrategy : MonoBehaviour {
	// TODO: There is a ton of boilerplate in the derived classes.
	// Look into DRYing it up here

	// TODO: Also consider passing a ColorPool instead of Color[]
	public abstract void AssignColors(DotGrid dots, Color[] colors);
}
