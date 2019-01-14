using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GridManager : MonoBehaviour {
	private static int WIDTH = 6;
	private static int PLAYABLE_HEIGHT = 6;
	private static int TOTAL_HEIGHT = PLAYABLE_HEIGHT * 2;
	private GameObject[,] dots = new GameObject[WIDTH, TOTAL_HEIGHT];

	public GameObject dotPrefab;

	public float dotScale = 1.0f;
	public float distanceBetweenDots = 1.0f;

	private List<GameObject> selectedDots = new List<GameObject>();

	private enum GameStates { Ready, DroppingDots };
	private GameStates gameState;

	public LineManager lineManager;

	void Start () {
		for (int j = 0; j < TOTAL_HEIGHT; j++) {
			for (int i = 0; i < WIDTH; i++) {
				CreateDot(i, j);
			}
		}

		gameState = GameStates.Ready;
	}

	private void CreateDot(int i, int j) {
		dots[i,j] = Instantiate(dotPrefab, new Vector3((float)i * distanceBetweenDots, (float)j * distanceBetweenDots), Quaternion.identity);
		dots[i,j].transform.localScale = new Vector2(dotScale, dotScale);

		if (j >= PLAYABLE_HEIGHT) {
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
			DropDots();
			gameState = GameStates.Ready;
		}
		else {
			throw new Exception("Invalid GameState");
		}
	}

	public List<GameObject> GetSelectedDots() {
		return selectedDots;
	}

	public GameObject GetCurrentDot() {
		if (selectedDots.Count > 0) {
			return selectedDots[selectedDots.Count - 1];
		}
		return null;
	}
	// TODO: See if this can easily be moved to another class or component
	private void HandleMouseClick() {
		if (Input.GetMouseButtonDown(0)) {
			Collider2D clickedDotCollider = GetColliderUnderMouseCursor();
			if (clickedDotCollider != null) {
				selectedDots.Add(clickedDotCollider.gameObject);
			}
		}
	}

	private void HandleMouseHold() {
		if (Input.GetMouseButton(0)) {
			Collider2D dotUnderCursorCollider = GetColliderUnderMouseCursor();

			if (dotUnderCursorCollider != null && selectedDots.Count > 0) {
				GameObject dotUnderCursor = dotUnderCursorCollider.gameObject;

				Vector2 coordinatesUnderCursor = GetCoordinatesOfDot(dotUnderCursor);
				Vector2 coordinatesOfLastSelectedDot = GetCoordinatesOfDot(GetLastSelectedDot());

				// TODO Look into breaking up this conditional, getting pretty unwieldy
				if (CoordinatesAreAdjacent(coordinatesUnderCursor, coordinatesOfLastSelectedDot)) {
					if (dotUnderCursor == GetSecondToLastSelectedDot()) {
						selectedDots.RemoveAt(selectedDots.Count - 1);
					}
					else {
						// TODO: Figure out how if there's a way around checking GetComponent so many times
						if (GetSelectedDotColor() == GetDotColor(dotUnderCursor)) {
							selectedDots.Add(dotUnderCursor);
						}
					}
				}
			}
		}
	}

	private void HandleMouseRelease() {
		if (Input.GetMouseButtonUp(0)) {
			if (selectedDots.Count > 1) {
				if (IsLoopSelected()) {
					RemoveAllDotsOfColor(GetSelectedDotColor());
				}
				else {
					RemoveSelectedDots();
				}

				DropDots();
				ReplenishDots();

				gameState = GameStates.DroppingDots;
			}

			selectedDots.Clear();
		}
	}

	private Collider2D GetColliderUnderMouseCursor() {
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector2 mousePosition2D = new Vector2(mousePosition.x, mousePosition.y);
		RaycastHit2D hit = Physics2D.Raycast(mousePosition2D, Vector2.zero);

		return hit.collider;
	}

	private Vector2 GetCoordinatesOfDot(GameObject dot) {
		for (int j = 0; j < PLAYABLE_HEIGHT; j++) {
			for (int i = 0; i < WIDTH; i++) {
				if (dots[i,j] == dot) {
					return new Vector2(i,j);
				}
			}
		}

		throw new Exception("Unable to find dot");
	}

	private GameObject GetLastSelectedDot() {
		if (selectedDots.Count > 0) {
			return selectedDots[selectedDots.Count - 1];
		}

		return null;
	}

	private GameObject GetSecondToLastSelectedDot() {
		if (selectedDots.Count > 1) {
			return selectedDots[selectedDots.Count - 2];
		}

		return null;
	}

	private bool CoordinatesAreAdjacent(Vector2 v1, Vector2 v2) {
		return ((int)Mathf.Abs(v1.x - v2.x) + (int)Mathf.Abs(v1.y - v2.y) == 1);
	}

	private bool IsLoopSelected() {
		HashSet<GameObject> uniqueSelectedDots = new HashSet<GameObject>();

		foreach (GameObject dot in selectedDots) {
			if (uniqueSelectedDots.Contains(dot)) {
				return true;
			}
			uniqueSelectedDots.Add(dot);
		}

		return false;
	}

	private void RemoveSelectedDots() {
		foreach (GameObject dot in selectedDots) {
			Destroy(dot);
		}
	}

	private void RemoveAllDotsOfColor(Color c) {
		for (int j = 0; j < PLAYABLE_HEIGHT; j++) {
			for (int i = 0; i < WIDTH; i++) {
				if (GetDotColor(dots[i,j]) == c) {
					Destroy(dots[i,j]);
				}
			}
		}
	}

	private Color GetSelectedDotColor() {
		return GetDotColor(selectedDots[0]);
	}

	private Color GetDotColor(GameObject dot) {
		if (dot != null) {
			// TODO: Again, figure out how if there's a way around checking GetComponent so many times
			return dot.GetComponent<SpriteRenderer>().color;
		}

		throw new Exception("Trying to get color for a dot that doesn't exist");
	}

	private void DropDots() {
		for (int j = 0; j < PLAYABLE_HEIGHT; j++) {
			for (int i = 0; i < WIDTH; i++) {
				if (dots[i,j] == null) {
					DropDot(i,j);
				}
			}
		}
	}

	private void DropDot(int i, int jDestination) {
		int jSource = jDestination;
		while (dots[i,jSource] == null && jSource < TOTAL_HEIGHT - 1) {
			jSource++;
		}
		if (dots[i,jSource] != null) {
			MoveDot(i, jDestination, jSource);
		}
	}

	private void MoveDot(int i, int jDestination, int jSource) {
		dots[i,jSource].SetActive(true);
		dots[i,jDestination] = dots[i,jSource];
		dots[i,jSource] = null;

		Vector3 startPosition = new Vector3((float)i * distanceBetweenDots, (float)jSource * distanceBetweenDots);
		Vector3 stopPosition = new Vector3((float)i * distanceBetweenDots, (float)jDestination * distanceBetweenDots);
		dots[i,jDestination].GetComponent<Dropper>().StartDropping(startPosition, stopPosition);
	}

	private void ReplenishDots() {
		// Because of the side of the buffer, the "playable" space should always be completely
		// filled before replenishing dots. Thus, should only need to replenish the "unplayable"
		// dots above the playable area
		for (int j = PLAYABLE_HEIGHT; j < TOTAL_HEIGHT; j++) {
			for (int i = 0; i < WIDTH; i++) {
				CreateDot(i, j);
			}
		}
	}
}
