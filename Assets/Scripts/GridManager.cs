﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GridManager : MonoBehaviour {
	public GameObject dotPrefab;
	public LineManager lineManager;

	public float dotScaleFactor = .5f;
	private Vector2 dotScale;

	public float distanceBetweenDots = 1.0f;

	private static int WIDTH = 6;
	private static int PLAYABLE_HEIGHT = 6;
	private static int TOTAL_HEIGHT = PLAYABLE_HEIGHT * 2;
	private GameObject[,] dots = new GameObject[WIDTH, TOTAL_HEIGHT];

	private List<Vector2Int> selectedDotIndices = new List<Vector2Int>();
	private Queue<GameObject> dotPool = new Queue<GameObject>();

	private enum GameStates { Ready, DroppingDots };
	private GameStates gameState;

	void Start () {
		dotScale = new Vector2(dotScaleFactor, dotScaleFactor);
		
		for (int j = 0; j < TOTAL_HEIGHT; j++) {
			for (int i = 0; i < WIDTH; i++) {
				CreateDot(i, j);
			}
		}

		gameState = GameStates.Ready;
	}

	private void CreateDot(int i, int j) {
		if (dotPool.Count > 0) {
			Debug.Log("Dequeuing");
			dots[i,j] = dotPool.Dequeue();
		}
		else {
			Debug.Log("Instantiating");
			dots[i,j] = Instantiate(dotPrefab, new Vector3((float)i * distanceBetweenDots, (float)j * distanceBetweenDots), Quaternion.identity);
		}
		
		dots[i,j].transform.localScale = dotScale;

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
			// Note that this doesn't wait for the dots to finish dropping before
			// returning the GameState to ready. This won't be a problem as long
			// as players aren't inhumanly fast or the drop speed isn't turned down
			// to something extremely low
			DropDots();
			ReplenishDots();
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
				if (selectedDotIndices.Count == 0) {
					lineManager.EnableLineToCursor();
					lineManager.UpdateLineToCursorColor(clickedDot.GetComponent<SpriteRenderer>().color);
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
							if (GetSelectedDotColor() == GetDotColor(dotUnderCursor)) {
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
				if (IsLoopSelected()) {
					RemoveAllDotsOfColor(GetSelectedDotColor());
				}
				else {
					RemoveSelectedDots();
				}

				gameState = GameStates.DroppingDots;
			}

			selectedDotIndices.Clear();
			lineManager.RemoveAllLines();
		}
	}

	private Vector2Int GetArrayCoordinatesOfDot(GameObject dot) {
		for (int j = 0; j < PLAYABLE_HEIGHT; j++) {
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

	private void RemoveSelectedDots() {
		foreach (Vector2Int coords in selectedDotIndices) {
			RemoveDotAtCoords(coords);
			//Destroy(dots[coords.x, coords.y]);
		}
	}

	private void RemoveAllDotsOfColor(Color c) {
		for (int j = 0; j < PLAYABLE_HEIGHT; j++) {
			for (int i = 0; i < WIDTH; i++) {
				if (GetDotColor(dots[i,j]) == c) {
					RemoveDotAtCoords(i,j);
					//Destroy(dots[i,j]);
				}
			}
		}
	}

	private void RemoveDotAtCoords(int x, int y) {
		RemoveDotAtCoords(new Vector2Int(x, y));
	}
	
	private void RemoveDotAtCoords(Vector2Int coords) {
		GameObject dot = DotAtCoords(coords);
		dot.SetActive(false);
		dotPool.Enqueue(dot);
		Debug.Log("Enqueueing");
		dots[coords.x, coords.y] = null;
	}

	// TODO: Move this elsewhere in the file
	private GameObject DotAtCoords(Vector2Int coords) {
		return dots[coords.x, coords.y];
	}

	private Color GetSelectedDotColor() {
		// TODO: Revisit how to do this. Does it make sense to set/unset this on click/release?
		// Does it make sense to have this BOTH here and the line manager (no).
		Vector2Int coords = selectedDotIndices[0];
		return GetDotColor(DotAtCoords(coords));
	}

	private Color GetDotColor(GameObject dot) {
		if (dot != null) {
			// TODO: Again, figure out how if there's a way around checking GetComponent so many times
			return dot.GetComponent<SpriteRenderer>().color;
		}

		throw new Exception("Trying to get color for a dot that doesn't exist");
	}

	private void DropDots() {
		for (int y = 0; y < PLAYABLE_HEIGHT; y++) {
			for (int x = 0; x < WIDTH; x++) {
				if (dots[x,y] == null) {
					DropDot(x,y);
				}
			}
		}
	}

	private void DropDot(int x, int yDestination) {
		int ySource = yDestination;
		while (dots[x,ySource] == null && ySource < TOTAL_HEIGHT - 1) {
			ySource++;
		}

		if (dots[x,ySource] != null) {
			MoveDot(x, yDestination, ySource);
		}
		else {
			throw new Exception("Unable to drop dot");
		}
	}

	private void MoveDot(int x, int yDestination, int ySource) {
		dots[x,ySource].SetActive(true);
		dots[x,yDestination] = dots[x,ySource];
		dots[x,ySource] = null;

		Vector3 startPosition = new Vector3((float)x * distanceBetweenDots, (float)ySource * distanceBetweenDots);
		Vector3 stopPosition = new Vector3((float)x * distanceBetweenDots, (float)yDestination * distanceBetweenDots);
		dots[x,yDestination].GetComponent<Dropper>().StartDropping(startPosition, stopPosition);
	}

	private void ReplenishDots() {
		// Because of the side of the buffer, the "playable" space should always be completely
		// filled before replenishing dots. Thus, should only need to replenish the "unplayable"
		// dots above the playable area
		for (int j = PLAYABLE_HEIGHT; j < TOTAL_HEIGHT; j++) {
			for (int i = 0; i < WIDTH; i++) {
				if (dots[i,j] == null) {
					CreateDot(i, j);
				}
			}
		}
	}
}