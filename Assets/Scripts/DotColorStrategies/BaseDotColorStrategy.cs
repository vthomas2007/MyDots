using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseDotColorStrategy : MonoBehaviour {
	public void AssignColors(DotGrid grid, ColorPool colors) {
		int columns = grid.Width();
		int rows = grid.Height();
		int startingRow = grid.PlayableHeight();

		PreLoop(colors);

		for (int y = startingRow; y < rows; y++) {
			for (int x = 0; x < columns; x++) {
				if (grid.CellIsOccupied(x, y)) {
					AssignColor(grid, colors, x, y);
				}
			}
		}

		PostLoop(colors);
	}

	protected virtual void PreLoop(ColorPool colors) { }
	protected abstract void AssignColor(DotGrid grid, ColorPool colors, int x, int y);
	protected virtual void PostLoop(ColorPool colors) { }
}
