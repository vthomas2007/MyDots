using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GridManager : MonoBehaviour {
	public GameObject dotPrefab;
	public LineManager lineManager;
	public CurrentColor currentColorStore;
	public ColorPool colorPool;

	public float dotScaleFactor = .5f;
	private Vector2 dotScale;

	public float distanceBetweenDots = 1.0f;

	public int WIDTH = 6;
	public int HEIGHT = 6;
	private int TOTAL_HEIGHT;

	private GameObject[,] dots;
	private List<Vector2Int> selectedDotIndices = new List<Vector2Int>();

	private enum GameStates { Ready, DroppingDots };
	private GameStates gameState;

	private float dropHeight;

	void Start() {
		TOTAL_HEIGHT = HEIGHT * 2;
		dots = new GameObject[WIDTH, TOTAL_HEIGHT];
		dotScale = new Vector2(dotScaleFactor, dotScaleFactor);
		
		for (int j = 0; j < HEIGHT; j++) {
			for (int i = 0; i < WIDTH; i++) {
				CreateDot(i, j);
				// TODO: Restructure this
				SetDotColor(dots[i,j]);
			}
		}

		gameState = GameStates.Ready;

		// TODO: Move to method or renderer component
		float horizontalBuffer = 1.0f;
		float verticalBuffer = 1.0f;
		float contentWidth = WIDTH * distanceBetweenDots + (2.0f * horizontalBuffer);
		float contentHeight = HEIGHT * distanceBetweenDots + (2.0f * verticalBuffer);

		Camera mainCamera = Camera.main;

		float minCameraSize = contentHeight;
		float cameraWidthWithMinCameraSize = minCameraSize * mainCamera.aspect;

		if (cameraWidthWithMinCameraSize < contentWidth) {
			float expandWidthAmount = contentWidth - cameraWidthWithMinCameraSize;
			float expandHeightAmount = expandWidthAmount / mainCamera.aspect;
			minCameraSize += expandHeightAmount;
		}

		minCameraSize *= 0.5f;

		mainCamera.orthographicSize = minCameraSize;
		
		float cameraX = WIDTH * distanceBetweenDots * 0.5f;
		float cameraY = HEIGHT * distanceBetweenDots * 0.5f;
		mainCamera.gameObject.transform.position = new Vector3(cameraX, cameraY, -1);

		dropHeight = (2 * minCameraSize) - distanceBetweenDots;
	}

	private void CreateDot(int i, int j) {
		dots[i,j] = Instantiate(dotPrefab, new Vector3((float)i * distanceBetweenDots, (float)j * distanceBetweenDots), Quaternion.identity);
		dots[i,j].transform.localScale = dotScale;

		if (j >= HEIGHT) {
			dots[i,j].SetActive(false);
		}
	}
	
	void Update () {
		if (gameState == GameStates.Ready) {
			HandleMouseClick();
			HandleMouseHold();
			HandleMouseRelease();
		}
		else if (gameState == GameStates.DroppingDots) {
			// Note that this doesn't wait for the dots to finish dropping before
			// returning the GameState to ready. This won't be a problem as long
			// as players aren't inhumanly fast or the drop speed isn't turned down
			// to something extremely low
			DropDots();
			//ReplenishDots();
			gameState = GameStates.Ready;
		}
		else {
			throw new Exception("Invalid GameState");
		}
	}

	// TODO: See if these can easily be moved to an InputHandler class or component
	private void HandleMouseClick() {
		if (Input.GetMouseButtonDown(0)) {
			GameObject clickedDot = GetDotUnderMouseCursor();

			if (clickedDot != null) {
				// TODO: Look into caching the color of the dots somewhere outside of a component
				currentColorStore.currentColor = clickedDot.GetComponent<SpriteRenderer>().color;

				if (selectedDotIndices.Count == 0) {
					lineManager.EnableLineToCursor();
				}

				selectedDotIndices.Add(GetArrayCoordinatesOfDot(clickedDot));
			}
		}
	}

	private GameObject GetDotUnderMouseCursor() {
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector2 mousePosition2D = new Vector2(mousePosition.x, mousePosition.y);
		RaycastHit2D hit = Physics2D.Raycast(mousePosition2D, Vector2.zero);

		return (hit.collider == null) ? null : hit.collider.gameObject;
	}

	private void HandleMouseHold() {
		// TODO Look into breaking up this conditional, getting pretty unwieldy
		if (Input.GetMouseButton(0)) {
			GameObject lastSelectedDot = GetLastSelectedDot();
			
			if (lastSelectedDot != null) {
				lineManager.DrawLineToCursorFromDot(lastSelectedDot);

				GameObject dotUnderCursor = GetDotUnderMouseCursor();
				if (dotUnderCursor != null) {
					
					Vector2Int arrayCoordinatesUnderCursor = GetArrayCoordinatesOfDot(dotUnderCursor);
					Vector2Int arrayCoordinatesOfLastSelectedDot = GetArrayCoordinatesOfDot(lastSelectedDot);

					if (CoordinatesAreAdjacent(arrayCoordinatesUnderCursor, arrayCoordinatesOfLastSelectedDot)) {
						if (dotUnderCursor == GetSecondToLastSelectedDot()) {
							Backtrack();
						}
						else {
							if (currentColorStore.currentColor == GetDotColor(dotUnderCursor)) {
								lineManager.AddLine(lastSelectedDot, dotUnderCursor);
								selectedDotIndices.Add(arrayCoordinatesUnderCursor);
							}
						}
					}
				}
			}
		}
	}

	private GameObject GetLastSelectedDot() {
		if (selectedDotIndices.Count > 0) {
			Vector2Int indices = selectedDotIndices[selectedDotIndices.Count - 1];
			return dots[indices.x, indices.y];
		}

		return null;
	}
	
	private void Backtrack() {
		selectedDotIndices.RemoveAt(selectedDotIndices.Count - 1);
		lineManager.RemoveLastLine();
	}

	private void HandleMouseRelease() {
		if (Input.GetMouseButtonUp(0)) {
			if (selectedDotIndices.Count > 1) {
				RemoveDots();
				gameState = GameStates.DroppingDots;
			}

			selectedDotIndices.Clear();
			lineManager.RemoveAllLines();
		}
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

	private List<Vector2Int> RemoveAllDotsOfColor(Color c) {
		List<Vector2Int> coordsList = new List<Vector2Int>();
		for (int y = 0; y < HEIGHT; y++) {
			for (int x = 0; x < WIDTH; x++) {
				if (GetDotColor(dots[x, y]) == c) {
					RemoveDotAtCoords(x, y);
				}
			}
		}
		return coordsList;
	}

	private void RemoveSelectedDots() {
		foreach (Vector2Int coords in selectedDotIndices) {
			RemoveDotAtCoords(coords);
		}
	}

	private Vector2Int GetArrayCoordinatesOfDot(GameObject dot) {
		for (int j = 0; j < HEIGHT; j++) {
			for (int i = 0; i < WIDTH; i++) {
				if (dots[i,j] == dot) {
					return new Vector2Int(i,j);
				}
			}
		}

		throw new Exception("Unable to find dot");
	}

	private GameObject GetSecondToLastSelectedDot() {
		if (selectedDotIndices.Count > 1) {
			Vector2Int coords = selectedDotIndices[selectedDotIndices.Count - 2];
			return dots[coords.x, coords.y];
		}

		return null;
	}

	private bool CoordinatesAreAdjacent(Vector2 v1, Vector2 v2) {
		return (int)(Mathf.Abs(v1.x - v2.x) + Mathf.Abs(v1.y - v2.y)) == 1;
	}

	private void RemoveDotAtCoords(int x, int y) {
		RemoveDotAtCoords(new Vector2Int(x, y));
	}
	
	private void RemoveDotAtCoords(Vector2Int coords) {
		GameObject dot = DotAtCoords(coords);
		dot.SetActive(false);
		int destinationRow = NextFreeRowInColumn(coords.x);
		dots[coords.x, destinationRow] = dot;
		dots[coords.x, coords.y] = null;
	}

	private int NextFreeRowInColumn(int columnIndex) {
		int y = HEIGHT;
		while (dots[columnIndex, y] != null) {
			Debug.Log(y.ToString());
			y++;
		}

		// TODO: Throw exception if exceeds TOTAL_HEIGHT
		return y;
	}

	// TODO: Move this elsewhere in the file
	private GameObject DotAtCoords(Vector2Int coords) {
		return dots[coords.x, coords.y];
	}

	private Color GetDotColor(GameObject dot) {
		if (dot != null) {
			// TODO: Again, figure out how if there's a way around checking GetComponent so many times
			return dot.GetComponent<SpriteRenderer>().color;
		}

		throw new Exception("Trying to get color for a dot that doesn't exist");
	}

	private void SetDotColor(GameObject dot) {
		int colorIndex = UnityEngine.Random.Range(0, colorPool.availableColors.Length);
		dot.GetComponent<SpriteRenderer>().color = colorPool.availableColors[colorIndex];
	}

	private void DropDots() {
		List<Vector2Int> coordsToRecolor = new List<Vector2Int>();

		for (int y = 0; y < HEIGHT; y++) {
			for (int x = 0; x < WIDTH; x++) {
				if (dots[x, y] == null) {
					DropDot(x,y);
				}
			}
		}
	}

	private int YIndexToDropFrom(int x, int y) {
		int ySource = y;

		while (dots[x, ySource] == null && ySource < TOTAL_HEIGHT - 1) {
			ySource++;
		}

		return ySource;
	}

	private void DropDot(int x, int y) {
		int ySource = YIndexToDropFrom(x, y);
		
		if (dots[x, ySource] != null) {
			MoveDot(x, y, ySource);
		}
		else {
			throw new Exception("Unable to drop dot");
		}
	}

	private void MoveDot(int x, int yDestination, int ySource) {
		dots[x, yDestination] = dots[x,ySource];
		dots[x, yDestination].SetActive(true);

		dots[x, ySource] = null;

		float startingY = ySource * distanceBetweenDots;
		if (ySource >= HEIGHT) {
			startingY += dropHeight;
		}

		Vector3 startPosition = new Vector3((float)x * distanceBetweenDots, startingY);
		Vector3 stopPosition = new Vector3((float)x * distanceBetweenDots, (float)yDestination * distanceBetweenDots);
		dots[x, yDestination].GetComponent<Dropper>().StartDropping(startPosition, stopPosition);
	}
}