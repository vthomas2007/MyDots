using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Assign all dots to a single color. Each time dots are dropped, rotate to the "next"
// color in the pool and start back at the beginning when all the colors have been
// used.
public class RotatingSingleColorStrategy : BaseDotColorStrategy {
	private int colorIndex = 0;

	protected override void AssignColor(DotGrid grid, ColorPool colors, int x, int y) {
		grid.SetColor(x, y, colors[colorIndex]);
	}

	protected override void PostLoop(ColorPool colors) {
		colorIndex = (colorIndex + 1) % colors.Count();
	}
}