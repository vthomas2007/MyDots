using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotCell {
	public GameObject dot { get; set; }
	private Color color;

	public void SetColor(Color c) {
		color = c;
		dot.GetComponentInChildren<SpriteRenderer>().color = c;
	}

	public Color Color() {
		return color;
	}
}