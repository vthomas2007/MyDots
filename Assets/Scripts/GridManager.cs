using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GridManager : MonoBehaviour {
	private static int WIDTH = 6;
	private static int HEIGHT = 6;
	private GameObject[,] dots = new GameObject[WIDTH, HEIGHT];

	public GameObject dotPrefab;

	public float dotScale = 1.0f;
	public float distanceBetweenDots = 1.0f;

	public float lineWidth = 0.25f;

	private List<GameObject> selectedDots = new List<GameObject>();

	// Use this for initialization
	void Start () {
		// TODO Move to method
		for (int i = 0; i < HEIGHT; i++) {
			for (int j = 0; j < WIDTH; j++) {
				dots[i,j] = Instantiate(dotPrefab, new Vector3((float)i * distanceBetweenDots, (float)j * distanceBetweenDots), Quaternion.identity);
				dots[i,j].transform.localScale = new Vector2(dotScale, dotScale);
			}
		}
	}
	
	void Update () {
		HandleMouseClick();
		HandleMouseHold();
		HandleMouseRelease();
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

			if (dotUnderCursorCollider != null) {
				GameObject dotUnderCursor = dotUnderCursorCollider.gameObject;

				if (selectedDots.Count > 0) {
					Vector2 indicesUnderCursor = GetIndicesOfDot(dotUnderCursor);
					Vector2 indicesOfLastSelectedDot = GetIndicesOfDot(GetLastSelectedDot());

					if (IndicesAreAdjacent(indicesUnderCursor, indicesOfLastSelectedDot) && !InSelectedSet(dotUnderCursor)) {
						// TODO: Figure out how if there's a way around checking GetComponent so many times
						if (GetLastSelectedDot().GetComponent<SpriteRenderer>().color == dotUnderCursor.GetComponent<SpriteRenderer>().color) {
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
				foreach (GameObject dot in selectedDots) {
					Destroy(dot);
				}
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

	private Vector2 GetIndicesOfDot(GameObject dot) {
		for (int i = 0; i < HEIGHT; i++) {
			for (int j = 0; j < WIDTH; j++) {
				if (dots[i,j] == dot) {
					return new Vector2(i, j);
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

	private bool IndicesAreAdjacent(Vector2 v1, Vector2 v2) {
		return ((int)Mathf.Abs(v1.x - v2.x) + (int)Mathf.Abs(v1.y - v2.y) == 1);
	}

	private bool InSelectedSet(GameObject dot) {
		return selectedDots.Contains(dot);
	}
}
