using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseDotColorStrategy : MonoBehaviour {
	public abstract void AssignColors(DotCell[,] dots, Color[] colors);
}
