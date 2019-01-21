using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotCell {
	public GameObject dot { get; set; }
	private Color color;

	// Keeping a "color" on the cell decouples the display logic from the
	// matching logic.
	public void SetColor(Color c) {
		color = c;
		// Leveraging DotColorUpdater avoids calling the even-more-expensive
		// GetComponentInChildren() function to get a handle on the renderer.
		dot.GetComponent<DotColorUpdater>().UpdateColor(c);
	}

	public Color Color() {
		return color;
	}
}