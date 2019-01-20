using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDotColorStrategy : BaseDotColorStrategy {
	protected override void AssignColor(DotGrid grid, ColorPool colors, int x, int y) {
		Color c = colors.GetRandomColor();
		grid.SetColor(x, y, c);
	}
}