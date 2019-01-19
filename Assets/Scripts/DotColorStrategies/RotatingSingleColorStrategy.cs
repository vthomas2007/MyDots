using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingSingleColorStrategy : BaseDotColorStrategy {
	private int colorIndex = 0;

	public override void AssignColors(DotCell[,] dots, Color[] colors) {
		int columns = dots.GetLength(0);
		int rows = dots.GetLength(1);
		int startingRow = rows / 2;

		for (int y = startingRow; y < rows; y++) {
			for (int x = 0; x < columns; x++) {
				if (dots[x, y].dot != null) {
					dots[x, y].SetColor(colors[colorIndex]);
				}
			}
		}

		colorIndex = (colorIndex + 1) % colors.Length;
	}
}