using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDotColorStrategy : BaseDotColorStrategy {
	public override void AssignColors(GameObject[,] dots, Color[] colors) {
		int columns = dots.GetLength(0);
		int rows = dots.GetLength(1);
		int startingRow = rows / 2;

		int colorCount = colors.Length;

		for (int y = startingRow; y < rows - 1; y++) {
			for (int x = 0; x < columns; x++) {
				if (dots[x, y] != null) {
					Color c = colors[Random.Range(0, colorCount)];
					dots[x, y].gameObject.GetComponent<SpriteRenderer>().color = c;
				}
			}
		}
	}
}
