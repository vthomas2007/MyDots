using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Picks a random color and assigns it to every dot that's about to be dropped
public class RandomSingleColorStrategy : BaseDotColorStrategy {
	private Color colorForAllDots;

	protected override void PreLoop(ColorPool colors) {
		colorForAllDots = colors.GetRandomColor();
	}

	protected override void AssignColor(DotGrid grid, ColorPool colors, int x, int y) {
		grid.SetColor(x, y, colorForAllDots);
	}
}