using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GridManager : MonoBehaviour {
	public GameObject dotPrefab;
	public CurrentColor currentColorStore;
	public LineManager lineManager;
	public ColorPool colorPool;

	public float dotScaleFactor = .5f;
	private Vector2 dotScale;

	// TODO: Decide if this belongs here or in the camera resizer
	public float distanceBetweenDots = 1.0f;
	private Camera mainCamera;

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
		dropHeight = (2 * mainCamera.orthographicSize) - distanceBetweenDots;

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
		mainCamera = Camera.main;
		mainCamera.GetComponent<CameraResizer>().Resize(WIDTH, HEIGHT, distanceBetweenDots);
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

		// TODO: Consider reworking this to use coords & leverage
		// the grid to get the color instead of reaching into the dot
		if (lastSelectedDot != null && DotsAreAdjacent(dot, lastSelectedDot)) {
			if (dot == GetSecondToLastSelectedDot()) {
				Backtrack();
			}
			else {
				if (currentColorStore.currentColor == GetDotColor(dot)) {
					Vector2Int arrayCoordinatesUnderCursor = grid.GetCoordinatesOfDot(dot);
					lineManager.AddLine(lastSelectedDot, dot);
					selectedDotIndices.Add(arrayCoordinatesUnderCursor);
				}
			}
		}
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
			RemoveAllDotsOfColor(currentColorStore.currentColor);
		}
		else {
			RemoveSelectedDots();
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

	// TODO: Determine if this belongs here or in the Grid class
	private List<Vector2Int> RemoveAllDotsOfColor(Color c) {
		List<Vector2Int> coordsList = new List<Vector2Int>();
		for (int y = 0; y < HEIGHT; y++) {
			for (int x = 0; x < WIDTH; x++) {
				if (grid.GetColor(x, y) == c) {
					grid.RemoveDotAtCoords(x, y);
				}
			}
		}
		return coordsList;
	}

	private void RemoveSelectedDots() {
		foreach (Vector2Int coords in selectedDotIndices) {
			grid.RemoveDotAtCoords(coords);
		}
	}

	private void AssignColorsToNewDots() {
		dotColorStrategy.AssignColors(grid, colorPool.availableColors);
	}

	private GameObject GetSecondToLastSelectedDot() {
		if (selectedDotIndices.Count > 1) {
			Vector2Int coords = selectedDotIndices[selectedDotIndices.Count - 2];
			return grid.GetDot(coords);
		}

		return null;
	}

	private bool DotsAreAdjacent(GameObject dot1, GameObject dot2) {
		Vector2Int v1 = grid.GetCoordinatesOfDot(dot1);
		Vector2Int v2 = grid.GetCoordinatesOfDot(dot2);

		return (int)(Mathf.Abs(v1.x - v2.x) + Mathf.Abs(v1.y - v2.y)) == 1;
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
					DropDot(x,y);
				}
			}
		}
	}

	private void DropDot(int x, int y) {
		int ySource = YIndexToDropFrom(x, y);
		
		if (grid.CellIsOccupied(x, ySource)) {
			MoveDot(x, y, ySource);
		}
		else {
			throw new Exception("Unable to drop dot");
		}
	}

	// TODO: Determine if this belongs in the grid or here
	private int YIndexToDropFrom(int x, int y) {
		int ySource = y;

		while (grid.CellIsEmpty(x, ySource) && ySource < TOTAL_HEIGHT - 1) {
			ySource++;
		}

		return ySource;
		// TODO: Throw exception if exceeds TOTAL_HEIGHT
	}

	// TODO: Swap source and destination param order
	private void MoveDot(int x, int yDestination, int ySource) {
		// TODO: The motivation for the grid refactor: Safe place to move dot AND colors
		grid.MoveDot(x, yDestination, ySource);

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