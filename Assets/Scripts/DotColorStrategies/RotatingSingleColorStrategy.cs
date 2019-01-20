using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingSingleColorStrategy : BaseDotColorStrategy {
	private int colorIndex = 0;

	public override void AssignColors(DotGrid grid, Color[] colors) {
		int columns = grid.Width();
		int rows = grid.Height();
		int startingRow = rows / 2;

		for (int y = startingRow; y < rows; y++) {
			for (int x = 0; x < columns; x++) {
				if (grid.CellIsOccupied(x, y)) {
					grid.SetColor(x, y, colors[colorIndex]);
				}
			}
		}

		colorIndex = (colorIndex + 1) % colors.Length;
	}
}