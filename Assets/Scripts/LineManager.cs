using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LineManager : MonoBehaviour {
	public GameObject lineRendererPrefab;
	public CurrentColor currentColorStore;

	public float lineWidth = 0.25f;

	private GameObject lineToCursorGameObject;
	private LineRenderer lineToCursorRenderer;

	private List<GameObject> lines;
	private Material lineMaterial;

	private Queue<GameObject> linePool = new Queue<GameObject>();
	
	void Start() {
		lines = new List<GameObject>();
		lineMaterial = new Material(Shader.Find("Sprites/Default"));

		InitializeLineToCursor();
	}

	private void InitializeLineToCursor() {
		lineToCursorGameObject = Instantiate(lineRendererPrefab, Vector3.zero, Quaternion.identity);

		lineToCursorRenderer = lineToCursorGameObject.GetComponent<LineRenderer>();
		lineToCursorRenderer.material = lineMaterial;
		lineToCursorRenderer.widthMultiplier = lineWidth;
		lineToCursorRenderer.enabled = false;
	}
	
	public void AddLineBetweenDots(GameObject sourceDot, GameObject destinationDot) {
		GameObject newLine;
		if (linePool.Count > 0) {
			newLine = linePool.Dequeue();
			newLine.SetActive(true);
		}
		else {
			newLine = Instantiate(lineRendererPrefab, Vector3.zero, Quaternion.identity);
		}

		LineRenderer lineRenderer = newLine.GetComponent<LineRenderer>();
		lineRenderer.material = lineMaterial;
		lineRenderer.widthMultiplier = lineWidth;

		lineRenderer.startColor = currentColorStore.currentColor;
		lineRenderer.endColor = currentColorStore.currentColor;

		Vector3[] points = new Vector3[2];
		points[0] = sourceDot.transform.position;
		points[1] = destinationDot.transform.position;

		lineRenderer.SetPositions(points);

		lines.Add(newLine);
	}

	public void DrawLineToCursorFromDot(GameObject dot) {
		Vector3 cursorCoords = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		cursorCoords.z = 0;

		Vector3[] points = new Vector3[2];

		points[0] = dot.transform.position;
		points[1] = cursorCoords;

		lineToCursorRenderer.SetPositions(points);
	}

	public void RemoveLastLine() {
		GameObject lineToRemove = lines[lines.Count - 1];
		lineToRemove.SetActive(false);
		linePool.Enqueue(lineToRemove);
		lines.RemoveAt(lines.Count - 1);
	}

	public void RemoveAllLines() {
		while (lines.Count > 0) {
			RemoveLastLine();
		}
		
		DisableLineToCursor();
	}

	public void EnableLineToCursor() {
		UpdateLineToCursorColor(currentColorStore.currentColor);
		SetLineToCursorEnabled(true);
	}

	private void UpdateLineToCursorColor(Color c) {
		lineToCursorRenderer.startColor = c;
		lineToCursorRenderer.endColor = c;
	}

	public void DisableLineToCursor() {
		SetLineToCursorEnabled(false);
	}

	private void SetLineToCursorEnabled(bool enabled) {
		lineToCursorRenderer.enabled = enabled;
	}
}