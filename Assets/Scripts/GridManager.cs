using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GridManager : MonoBehaviour {
	public GameObject dotPrefab;
	public LineManager lineManager;private static int WIDTH = 6;
	public LineRenderer lineToCursor;

	private static int PLAYABLE_HEIGHT = 6;
	private static int TOTAL_HEIGHT = PLAYABLE_HEIGHT * 2;
	private GameObject[,] dots = new GameObject[WIDTH, TOTAL_HEIGHT];

	public float dotScale = 1.0f;
	public float distanceBetweenDots = 1.0f;

	private List<GameObject> selectedDots = new List<GameObject>();

	private enum GameStates { Ready, DroppingDots };
	private GameStates gameState;

	void Start () {
		for (int j = 0; j < TOTAL_HEIGHT; j++) {
			for (int i = 0; i < WIDTH; i++) {
				CreateDot(i, j);
			}
		}

		// TODO: Move to variable, share with lineManager
		// OR just consider moving all of this to linemanager
		lineToCursor.widthMultiplier = 0.25f;
		lineToCursor.enabled = false;
		// TODO: Figure out if I need to remove these materials after removing lines
		lineToCursor.material = new Material(Shader.Find("Sprites/Default"));

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

				// TODO Again, this can probably all go to the line manager
				UpdateLineColor(clickedDotCollider.gameObject.GetComponent<SpriteRenderer>().color);
				lineToCursor.enabled = true;
			}
		}
	}

	private void UpdateLineColor(Color c) {
		// TODO: Only need to create the alphakey array once, can share between linemanager
		// This whole thing can be more efficient, don't need to keep instantiating Vector3s
		// for endpoint either, or the array of coords.

		/* Gradient gradient = new Gradient();
		gradient.SetKeys(
			new GradientColorKey[] { new GradientColorKey(c, 0.0f), new GradientColorKey(c, 1.0f) },
			new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
		);
		lineToCursor.colorGradient = gradient;
		*/
		lineToCursor.startColor = c;
		lineToCursor.endColor = c;
	}

	private void HandleMouseHold() {
		// TODO Look into breaking up this conditional, getting pretty unwieldy
		if (Input.GetMouseButton(0)) {
			GameObject lastSelectedDot = GetLastSelectedDot();
			
			if (lastSelectedDot != null) {
				DrawLineToCursorFromDot(lastSelectedDot);

				Collider2D dotUnderCursorCollider = GetColliderUnderMouseCursor();
				if (dotUnderCursorCollider != null) {
					
					GameObject dotUnderCursor = dotUnderCursorCollider.gameObject;

					Vector2 coordinatesUnderCursor = GetCoordinatesOfDot(dotUnderCursor);
					Vector2 coordinatesOfLastSelectedDot = GetCoordinatesOfDot(lastSelectedDot);

					if (CoordinatesAreAdjacent(coordinatesUnderCursor, coordinatesOfLastSelectedDot)) {
						if (dotUnderCursor == GetSecondToLastSelectedDot()) {
							selectedDots.RemoveAt(selectedDots.Count - 1);
							lineManager.RemoveLastLine();
						}
						else {
							if (GetSelectedDotColor() == GetDotColor(dotUnderCursor)) {
								lineManager.AddLine(lastSelectedDot, dotUnderCursor);
								selectedDots.Add(dotUnderCursor);
							}
						}
					}
				}
			}
		}
	}

	private void DrawLineToCursorFromDot(GameObject dot) {
		Vector3 cursorCoords = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		cursorCoords.z = 0;

		Vector3[] points = new Vector3[2];
		points[0] = dot.transform.position;
		points[1] = cursorCoords;

		lineToCursor.SetPositions(points);
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
			lineManager.ClearLines();
			HideLineToCursor();
		}
	}

	private void HideLineToCursor() {
		lineToCursor.enabled = false;
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
