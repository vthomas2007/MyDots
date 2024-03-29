﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class DotsGameManager : MonoBehaviour {
	public GameObject dotPrefab;
	public CurrentColor currentColorStore;
	public LineManager lineManager;
	public ColorPool colorPool;

	public float dotScaleFactor = .5f;
	public float distanceBetweenDots = 1.0f;

	public float delayBetweenDrops = 0.1f;
	public float dropDuration = 0.25f;

	public GameObject gridCamera;

	public int WIDTH = 6;
	public int HEIGHT = 6;
	private int TOTAL_HEIGHT;

	private DotGrid grid;
	private List<Vector2Int> connectedDotCoords = new List<Vector2Int>();

	private float dropHeight;

	public BaseDotColorStrategy initialDotColorStrategy;
	public BaseDotColorStrategy refillDotColorStrategy;

	void Start() {
		TOTAL_HEIGHT = HEIGHT * 2;

		ValidateColorPool();
		InitializeDots();
		InitializeDotColorStrategies();
		AssignColorsToNewDots(initialDotColorStrategy);
		InitializeCamera();
		CalculateDropHeight();
		DropDots();
	}

	private void ValidateColorPool() {
		if (colorPool == null || colorPool.Count() == 0) {
			throw new Exception("Color pool has not been assigned or has no colors");
		}
	}

	private void InitializeDots() {
		grid = new DotGrid(WIDTH, TOTAL_HEIGHT);

		Vector2 dotScale = new Vector2(dotScaleFactor, dotScaleFactor);
		for (int y = HEIGHT; y < TOTAL_HEIGHT; y++) {
			for (int x = 0; x < WIDTH; x++) {
				CreateDot(x, y, dotScale);
			}
		}
	}

	private void CreateDot(int x, int y, Vector2 scale) {
		GameObject newDot = Instantiate(dotPrefab, new Vector3((float)x * distanceBetweenDots, (float)y * distanceBetweenDots), Quaternion.identity);
		newDot.transform.localScale = scale;

		if (y >= HEIGHT) {
			newDot.SetActive(false);
		}

		grid.AddDot(x, y, newDot);
	}

	private void InitializeDotColorStrategies() {
		if (initialDotColorStrategy == null) {
			initialDotColorStrategy = gameObject.AddComponent<RandomDotColorStrategy>();
		}

		if (refillDotColorStrategy == null) {
			refillDotColorStrategy = gameObject.AddComponent<RandomDotColorStrategy>();
		}
	}

	private void AssignColorsToNewDots(BaseDotColorStrategy colorStrategy) {
		colorStrategy.AssignColors(grid, colorPool);
	}

	private void InitializeCamera() {
		gridCamera.GetComponent<CameraResizer>().Resize(WIDTH, HEIGHT, distanceBetweenDots);
	}

	private void CalculateDropHeight() {
		// An offset that is used when dropping new dots to ensure they always get
		// dropped from above the "top" of the screen
		dropHeight = (2 * gridCamera.GetComponent<Camera>().orthographicSize) - distanceBetweenDots;
	}

	private void DropDots() {
		int[] dotsDroppedPerColumn = new int[WIDTH];
		for (int x = 0; x < dotsDroppedPerColumn.Length; x++) {
			dotsDroppedPerColumn[x] = 0;
		}

		for (int y = 0; y < HEIGHT; y++) {
			for (int x = 0; x < WIDTH; x++) {
				if (grid.CellIsEmpty(x, y)) {
					DropDotIntoCoords(x, y, dotsDroppedPerColumn[x]);
					dotsDroppedPerColumn[x]++;
				}
			}
		}
	}

	private void DropDotIntoCoords(int x, int yDestination, int dotsDroppedInColumnSoFar) {
		int ySource = YCoordinateOfNextHighestDot(x, yDestination);

		if (grid.CellIsOccupied(x, ySource)) {
			MoveDot(x, ySource, yDestination, dotsDroppedInColumnSoFar);
		}
		else {
			throw new Exception("Unable to find dot to drop.");
		}
	}

	private int YCoordinateOfNextHighestDot(int x, int y) {
		int ySource = y;

		while (grid.CellIsEmpty(x, ySource) && ySource < TOTAL_HEIGHT) {
			ySource++;
		}

		return ySource;
	}

	private void MoveDot(int x, int ySource, int yDestination, int dotsDroppedInColumnSoFar) {
		grid.MoveDot(x, ySource, yDestination);

		Dropper.AnimationType animationType;
		float startingY = ySource * distanceBetweenDots;
		if (ySource >= HEIGHT) {
			startingY += dropHeight;
			animationType = Dropper.AnimationType.LargeBounce;
		}
		else {
			animationType = Dropper.AnimationType.SmallBounce;
		}

		float dropDelay = dotsDroppedInColumnSoFar * delayBetweenDrops;
		Vector3 startPosition = new Vector3((float)x * distanceBetweenDots, startingY);
		Vector3 stopPosition = new Vector3((float)x * distanceBetweenDots, (float)yDestination * distanceBetweenDots);

		grid.GetDot(x, yDestination).GetComponent<Dropper>().Drop(
			startPosition,
			stopPosition,
			animationType,
			dropDuration,
			dropDelay
		);
	}

	public void SelectDot(GameObject dot) {
		Vector2Int coords = grid.GetCoordinatesOfDot(dot);
		connectedDotCoords.Add(coords);

		currentColorStore.currentColor = grid.GetColor(coords.x, coords.y);

		lineManager.EnableLineToCursor();
	}

	public void UpdateLineToCursor() {
		GameObject lastSelectedDot = GetLastSelectedDot();
		
		if (lastSelectedDot != null) {
			lineManager.DrawLineToCursorFromDot(lastSelectedDot);
		}
	}

	private GameObject GetLastSelectedDot() {
		if (connectedDotCoords.Count > 0) {
			Vector2Int coords = connectedDotCoords[connectedDotCoords.Count - 1];
			return grid.GetDot(coords.x, coords.y);
		}

		return null;
	}

	public void ConnectOrDisconnect(GameObject dot) {
		GameObject lastSelectedDot = GetLastSelectedDot();

		if (lastSelectedDot != null && grid.DotsAreAdjacent(dot, lastSelectedDot)) {
			if (dot == SecondToLastSelectedDot()) {
				DisconnectLastDot();
			}
			else {
				ConnectIfEligible(dot);
			}
		}
	}

	private GameObject SecondToLastSelectedDot() {
		if (connectedDotCoords.Count > 1) {
			Vector2Int coords = connectedDotCoords[connectedDotCoords.Count - 2];
			return grid.GetDot(coords);
		}

		return null;
	}

	private void DisconnectLastDot() {
		connectedDotCoords.RemoveAt(connectedDotCoords.Count - 1);
		lineManager.RemoveLastLine();
	}

	private void ConnectIfEligible(GameObject dot) {
		Vector2Int dotCoordinates = grid.GetCoordinatesOfDot(dot);
		Color dotColor = grid.GetColor(dotCoordinates);

		Vector2Int lastSelectedCoordinates = grid.GetCoordinatesOfDot(GetLastSelectedDot());

		if (CurrentColor() == dotColor && !lineManager.LineExistsBetweenCoordinates(dotCoordinates, lastSelectedCoordinates)) {
			lineManager.AddLine(GetLastSelectedDot(), dot, lastSelectedCoordinates, dotCoordinates);
			connectedDotCoords.Add(dotCoordinates);
		}
	}

	private Color CurrentColor() {
		return currentColorStore.currentColor;
	}
	
	public void RemoveAndDropDots() {
		if (connectedDotCoords.Count > 1) {
			RemoveDots();
			AssignColorsToNewDots(refillDotColorStrategy);
			DropDots();
		}

		connectedDotCoords.Clear();
		lineManager.RemoveAllLines();
	}

	private void RemoveDots() {
		if (IsLoopSelected()) {
			grid.RemoveAllDotsOfColor(CurrentColor());
		}
		else {
			grid.RemoveDots(connectedDotCoords);
		}
	}

	private bool IsLoopSelected() {
		HashSet<Vector2Int> uniqueSelectedDots = new HashSet<Vector2Int>();

		foreach (Vector2Int coords in connectedDotCoords) {
			if (uniqueSelectedDots.Contains(coords)) {
				return true;
			}
			uniqueSelectedDots.Add(coords);
		}

		return false;
	}
}