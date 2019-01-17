using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LineManager : MonoBehaviour {
	public GameObject lineRendererPrefab;
	public float lineWidth = 0.25f;

	private Color currentlySelectedColor;

	private GameObject lineToCursorGameObject;
	private LineRenderer lineToCursor;
	private Vector3 lineToCursorCoords;

	private List<GameObject> lines;
	private Material lineMaterial;

	private Queue<GameObject> linePool = new Queue<GameObject>();
	
	// TODO: Consider using events. But may not surface enough information about selected dots etc.
	void Start () {
		lines = new List<GameObject>();
		lineMaterial = new Material(Shader.Find("Sprites/Default"));

		lineToCursorGameObject = Instantiate(lineRendererPrefab, Vector3.zero, Quaternion.identity);

		lineToCursor = lineToCursorGameObject.GetComponent<LineRenderer>();
		lineToCursor.material = lineMaterial;
		lineToCursor.widthMultiplier = lineWidth;
		lineToCursor.enabled = false;
	}
	
	public void AddLine(GameObject sourceDot, GameObject destinationDot) {
		GameObject newLine;
		if (linePool.Count > 0) {
			newLine = linePool.Dequeue();
			newLine.SetActive(true);
		}
		else {
			newLine = Instantiate(lineRendererPrefab, Vector3.zero, Quaternion.identity);
		}

		// TODO: Figure out a way to avoid GetComponent if possible
		LineRenderer lineRenderer = newLine.GetComponent<LineRenderer>();
		lineRenderer.material = lineMaterial;
		lineRenderer.widthMultiplier = lineWidth;

		lineRenderer.startColor = currentlySelectedColor;
		lineRenderer.endColor = currentlySelectedColor;

		Vector3[] points = new Vector3[2];
		points[0] = sourceDot.transform.position;
		points[1] = destinationDot.transform.position;

		lineRenderer.SetPositions(points);

		lines.Add(newLine);
	}

	public void DrawLineToCursorFromDot(GameObject dot) {
		lineToCursorCoords = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		lineToCursorCoords.z = 0;

		Vector3[] points = new Vector3[2];

		points[0] = dot.transform.position;
		points[1] = lineToCursorCoords;

		lineToCursor.SetPositions(points);
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

	public void UpdateLineToCursorColor(Color c) {
		currentlySelectedColor = c;

		lineToCursor.startColor = currentlySelectedColor;
		lineToCursor.endColor = currentlySelectedColor;
	}

	public void EnableLineToCursor() {
		SetLineToCursorEnabled(true);
	}

	public void DisableLineToCursor() {
		SetLineToCursorEnabled(false);
	}

	private void SetLineToCursorEnabled(bool enabled) {
		lineToCursor.enabled = enabled;
	}
}
