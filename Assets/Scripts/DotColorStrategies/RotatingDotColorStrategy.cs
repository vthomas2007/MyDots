using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Rotate between every color in the pool when assigning individual dot colors.
public class RotatingDotColorStrategy : BaseDotColorStrategy {
	private int colorIndex = 0;

	protected override void AssignColor(DotGrid grid, ColorPool colors, int x, int y) {
		grid.SetColor(x, y, colors[colorIndex]);
		colorIndex = (colorIndex + 1) % colors.Count();
	}
}
