using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; // for Exception

public class DotGrid {
	private DotCell[,] grid;
	private int gridWidth;
	private int gridHeight;
	private int playableHeight;

	public DotGrid(int width, int height) {
		grid = new DotCell[width, height];

		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				grid[x, y] = new DotCell();
			}
		}

		gridWidth = width;
		gridHeight = height;
		playableHeight = gridHeight / 2;
	}

	public void AddDot(int x, int y, GameObject dot, Color color) {
		Cell(x, y).dot = dot;
		Cell(x, y).SetColor(color);
	}

	public Color GetColor(Vector2Int coords) {
		return GetColor(coords.x, coords.y);
	}

	public Color GetColor(int x, int y) {
		return Cell(x, y).Color();
	}

	public void SetColor(int x, int y, Color c) {
		Cell(x, y).SetColor(c);
	}
	
	public int Width() {
		return gridWidth;
	}

	public int Height() {
		return gridHeight;
	}

	public int PlayableHeight() {
		return playableHeight;
	}

	public GameObject GetDot(int x, int y) {
		return Cell(x, y).dot;
	}

	public GameObject GetDot(Vector2Int coords) {
		return GetDot(coords.x, coords.y);
	}

	// TODO: Determine if it makes sense for this grid to "know" about the
	// distinction between playable and total height.
	// One option would be an optional parameter that caps the  
	public Vector2Int GetCoordinatesOfDot(GameObject dot) {
		for (int y = 0; y < Height(); y++) {
			for (int x = 0; x < Width(); x++) {
				if (GetDot(x, y) == dot) {
					return new Vector2Int(x, y);
				}
			}
		}

		throw new Exception("Unable to find dot");
	}
	
	public bool DotsAreAdjacent(GameObject dot1, GameObject dot2) {
		Vector2Int v1 = GetCoordinatesOfDot(dot1);
		Vector2Int v2 = GetCoordinatesOfDot(dot2);

		return (int)(Mathf.Abs(v1.x - v2.x) + Mathf.Abs(v1.y - v2.y)) == 1;
	}

	public void RemoveDotAtCoords(Vector2Int coords) {
		RemoveDotAtCoords(coords.x, coords.y);
	}

	public void RemoveAllDotsOfColor(Color c) {
		List<Vector2Int> coordsList = new List<Vector2Int>();
		for (int y = 0; y < PlayableHeight(); y++) {
			for (int x = 0; x < Width(); x++) {
				if (GetColor(x, y) == c) {
					RemoveDotAtCoords(x, y);
				}
			}
		}
	}

	public void RemoveDots(List<Vector2Int> dotCoords) {
		foreach (Vector2Int coords in dotCoords) {
			RemoveDotAtCoords(coords);
		}
	}

	public void RemoveDotAtCoords(int x, int y) {
		GameObject dot = GetDot(x, y);
		dot.SetActive(false);

		// Recycle dot by moving it to the top of the column
		// (which should always work out to be above the playable
		// area)
		int destinationRow = FirstEmptyRowInColum(x);

		Cell(x, destinationRow).dot = dot;
		Cell(x, y).dot = null;
	}

	private int FirstEmptyRowInColum(int x) {
		int y = PlayableHeight();
		while (CellIsOccupied(x, y)) {
			y++;
		}

		return y;
	}
	
	public bool CellIsEmpty(int x, int y) {
		return Cell(x, y).dot == null;
	}

	public bool CellIsOccupied(int x, int y) {
		return !CellIsEmpty(x, y);
	}

	public void MoveDot(int x, int ySource, int yDestination) {
		Cell(x, yDestination).dot = Cell(x, ySource).dot;
		Cell(x, yDestination).dot.SetActive(true);
		Cell(x, yDestination).SetColor(Cell(x, ySource).Color());

		Cell(x, ySource).dot = null;
	}

	private DotCell Cell(int x, int y) {
		return grid[x, y];
	}
}