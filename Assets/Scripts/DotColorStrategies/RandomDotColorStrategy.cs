using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDotColorStrategy : BaseDotColorStrategy {
	public override void AssignColors(DotGrid grid, Color[] colors) {
		int columns = grid.Width();
		int rows = grid.Height();
		int startingRow = rows / 2;

		int colorCount = colors.Length;

		for (int y = startingRow; y < rows; y++) {
			for (int x = 0; x < columns; x++) {
				if (grid.CellIsOccupied(x, y)) {
					Color c = colors[Random.Range(0, colorCount)];
					grid.SetColor(x, y, c);
				}
			}
		}
	}
}
