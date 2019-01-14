using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LineManager : MonoBehaviour {
	public GameObject lineRendererPrefab;
	public float lineWidth = 0.25f;

	private GameObject currentDot;
	private Vector3 start;
	private Vector3 end;

	private GameObject lineToCursorGameObject;
	private LineRenderer lineToCursor;
	private Vector3 lineToCursorCoords;

	private List<GameObject> lines;
	private Material lineMaterial;
	
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
	
	public void AddLine(GameObject dot1, GameObject dot2) {
		GameObject newLine = Instantiate(lineRendererPrefab, Vector3.zero, Quaternion.identity);
		// TODO: Figure out a way to avoid GetComponent if possible
		LineRenderer lineRenderer = newLine.GetComponent<LineRenderer>();
		lineRenderer.material = lineMaterial;
		lineRenderer.widthMultiplier = lineWidth;

		Color lineColor = dot1.gameObject.GetComponent<SpriteRenderer>().color;
		lineRenderer.startColor = lineColor;
		lineRenderer.endColor = lineColor;

		Vector3[] points = new Vector3[2];
		points[0] = dot1.transform.position;
		points[1] = dot2.transform.position;

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
		Destroy(lines[lines.Count - 1]);
		lines.RemoveAt(lines.Count - 1);
	}

	public void RemoveAllLines() {
		foreach (GameObject line in lines) {
			Destroy(line);
		}
		
		lines.Clear();
		SetLineToCursorEnabled(false);
	}

	public void UpdateLineToCursorColor(Color c) {
		lineToCursor.startColor = c;
		lineToCursor.endColor = c;
	}

	public void SetLineToCursorEnabled(bool enabled) {
		lineToCursor.enabled = enabled;
	}
}
