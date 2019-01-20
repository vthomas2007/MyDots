using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomSingleDotColorStrategy : BaseDotColorStrategy {
	public override void AssignColors(DotGrid grid, Color[] colors) {
		int columns = grid.Width();
		int rows = grid.Height();
		int startingRow = rows / 2;

		int colorCount = colors.Length;
		Color c = colors[Random.Range(0, colorCount)];

		for (int y = startingRow; y < rows; y++) {
			for (int x = 0; x < columns; x++) {
				if (grid.CellIsOccupied(x, y)) {
					grid.SetColor(x, y, c);
				}
			}
		}
	}
}