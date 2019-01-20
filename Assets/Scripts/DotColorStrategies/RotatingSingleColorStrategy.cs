using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingSingleColorStrategy : BaseDotColorStrategy {
	private int colorIndex = 0;

	protected override void AssignColor(DotGrid grid, ColorPool colors, int x, int y) {
		grid.SetColor(x, y, colors.availableColors[colorIndex]);
	}

	protected override void PostLoop(ColorPool colors) {
		colorIndex = (colorIndex + 1) % colors.Count();
	}
}