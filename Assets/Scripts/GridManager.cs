using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

// TODO Rename
public class GridManager : MonoBehaviour {
	public GameObject dotPrefab;
	public CurrentColor currentColorStore;
	public LineManager lineManager;
	public ColorPool colorPool;

	public float dotScaleFactor = .5f;
	private Vector2 dotScale;

	public float distanceBetweenDots = 1.0f;

	public GameObject gridCamera;

	public int WIDTH = 6;
	public int HEIGHT = 6;
	private int TOTAL_HEIGHT;

	private DotGrid grid;
	private List<Vector2Int> selectedDotIndices = new List<Vector2Int>();

	private float dropHeight;

	private BaseDotColorStrategy dotColorStrategy;

	void Start() {
		dotScale = new Vector2(dotScaleFactor, dotScaleFactor);

		TOTAL_HEIGHT = HEIGHT * 2;
		grid = new DotGrid(WIDTH, TOTAL_HEIGHT);

		for (int y = 0; y < HEIGHT; y++) {
			for (int x = 0; x < WIDTH; x++) {
				CreateDot(x, y);
			}
		}

		InitializeDotColorStrategy();
		InitializeCamera();
		// TODO: Figure out where to put this
		dropHeight = (2 * gridCamera.GetComponent<Camera>().orthographicSize) - distanceBetweenDots;

		// TODO: Look into initializing array above dots and then dropping all of them
	}

	private void CreateDot(int x, int y) {
		GameObject newDot = Instantiate(dotPrefab, new Vector3((float)x * distanceBetweenDots, (float)y * distanceBetweenDots), Quaternion.identity);
		newDot.transform.localScale = dotScale;

		if (y >= HEIGHT) {
			newDot.SetActive(false);
		}

		grid.AddDot(x, y, newDot, colorPool.GetRandomColor());
	}

	private void InitializeCamera() {
		gridCamera.GetComponent<CameraResizer>().Resize(WIDTH, HEIGHT, distanceBetweenDots);
	}

	private void InitializeDotColorStrategy() {
		dotColorStrategy = gameObject.GetComponent<BaseDotColorStrategy>();
		
		if (dotColorStrategy == null) {
			dotColorStrategy = gameObject.AddComponent<RandomDotColorStrategy>();
		}
	}

	public void SelectDot(GameObject dot) {
		Vector2Int coords = grid.GetCoordinatesOfDot(dot);
		selectedDotIndices.Add(coords);

		currentColorStore.currentColor = grid.GetColor(coords.x, coords.y);

		lineManager.EnableLineToCursor();
	}

	public void UpdateLineToCursor() {
		GameObject lastSelectedDot = GetLastSelectedDot();
		
		if (lastSelectedDot != null) {
			lineManager.DrawLineToCursorFromDot(lastSelectedDot);
		}
	}

	public void AddToOrRemoveFromSelectedList(GameObject dot) {
		GameObject lastSelectedDot = GetLastSelectedDot();

		if (lastSelectedDot != null && grid.DotsAreAdjacent(dot, lastSelectedDot)) {
			if (dot == SecondToLastSelectedDot()) {
				Backtrack();
			}
			else {
				AddToSelectedListIfColorMatches(dot);
			}
		}
	}

	private void AddToSelectedListIfColorMatches(GameObject dot) {
		Vector2Int dotCoordinates = grid.GetCoordinatesOfDot(dot);
		Color dotColor = grid.GetColor(dotCoordinates);

		if (CurrentColor() == dotColor) {
			lineManager.AddLine(GetLastSelectedDot(), dot);
			selectedDotIndices.Add(dotCoordinates);
		}
	}

	private Color CurrentColor() {
		return currentColorStore.currentColor;
	}

	private GameObject GetLastSelectedDot() {
		if (selectedDotIndices.Count > 0) {
			Vector2Int coords = selectedDotIndices[selectedDotIndices.Count - 1];
			return grid.GetDot(coords.x, coords.y);
		}

		return null;
	}
	
	private void Backtrack() {
		selectedDotIndices.RemoveAt(selectedDotIndices.Count - 1);
		lineManager.RemoveLastLine();
	}

	public void RemoveAndDropDots() {
		if (selectedDotIndices.Count > 1) {
			RemoveDots();
			AssignColorsToNewDots();
			DropDots();
		}

		selectedDotIndices.Clear();
		lineManager.RemoveAllLines();
	}

	private void RemoveDots() {
		if (IsLoopSelected()) {
			grid.RemoveAllDotsOfColor(CurrentColor());
		}
		else {
			grid.RemoveDots(selectedDotIndices);
		}
	}

	private bool IsLoopSelected() {
		HashSet<Vector2Int> uniqueSelectedDots = new HashSet<Vector2Int>();

		foreach (Vector2Int indices in selectedDotIndices) {
			if (uniqueSelectedDots.Contains(indices)) {
				return true;
			}
			uniqueSelectedDots.Add(indices);
		}

		return false;
	}

	private void AssignColorsToNewDots() {
		dotColorStrategy.AssignColors(grid, colorPool);
	}

	private GameObject SecondToLastSelectedDot() {
		if (selectedDotIndices.Count > 1) {
			Vector2Int coords = selectedDotIndices[selectedDotIndices.Count - 2];
			return grid.GetDot(coords);
		}

		return null;
	}

	private Color GetDotColor(GameObject dot) {
		if (dot != null) {
			// TODO: Again, figure out how if there's a way around checking GetComponent so many times
			return dot.GetComponent<SpriteRenderer>().color;
		}

		throw new Exception("Trying to get color for a dot that doesn't exist");
	}

	private void DropDots() {
		for (int y = 0; y < HEIGHT; y++) {
			for (int x = 0; x < WIDTH; x++) {
				if (grid.CellIsEmpty(x, y)) {
					DropDotIntoCoords(x, y);
				}
			}
		}
	}

	private void DropDotIntoCoords(int x, int yDestination) {
		int ySource = YIndexToDropFrom(x, yDestination);
		
		if (grid.CellIsOccupied(x, ySource)) {
			MoveDot(x, ySource, yDestination);
		}
		else {
			throw new Exception("Unable to drop dot");
		}
	}

	private int YIndexToDropFrom(int x, int y) {
		int ySource = y;

		while (grid.CellIsEmpty(x, ySource) && ySource < TOTAL_HEIGHT) {
			ySource++;
		}

		return ySource;
	}

	private void MoveDot(int x, int ySource, int yDestination) {
		grid.MoveDot(x, ySource, yDestination);

		float startingY = ySource * distanceBetweenDots;
		if (ySource >= HEIGHT) {
			startingY += dropHeight;
		}

		Vector3 startPosition = new Vector3((float)x * distanceBetweenDots, startingY);
		Vector3 stopPosition = new Vector3((float)x * distanceBetweenDots, (float)yDestination * distanceBetweenDots);
		// TODO: See if this GetComponent call is necessary
		grid.GetDot(x, yDestination).GetComponent<Dropper>().Drop(startPosition, stopPosition);
	}
}