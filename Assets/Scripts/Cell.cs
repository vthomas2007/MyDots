using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell  {
	public GameObject dotClass;
	public GameObject dot;

	Cell() {
		AddRandomDot();
	}

	private void AddRandomDot() {
		ColorRandomizer.GetRandomColor();
		dot = Instantiate(dotClass);
	}
}
