using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotCell {
	public GameObject dot { get; set; }

	// Keeping "color" on the cell decouples the matching logic to determine if
	// dots can be connected from display logic
	//
	// One benefit of this is it speeds up certain operations, such as DotGrid's 
	// RemoveAllDotsOfColor(), since it doesn't need to fetch the SpriteRenderer
	// of the underlying dots to determine their colors.
	private Color color;

	public void SetColor(Color c) {
		color = c;
		// Leveraging DotColorUpdater avoids calling the even-more-expensive
		// GetComponentInChildren() function to get a handle on the SpriteRenderer.
		dot.GetComponent<DotColorUpdater>().UpdateColor(c);
	}

	public Color Color() {
		return color;
	}
}