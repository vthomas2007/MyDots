using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LineManager : MonoBehaviour {
	public GameObject lineRendererPrefab;

	private GameObject currentDot;
	private Vector3 start;
	private Vector3 end;

	private List<GameObject> lines;
	
	// TODO: Consider using events. But may not surface enough information about selected dots etc.
	// Also, will need to maintain a list of gameobjects. These should probably just include the "fixed"
	// endpoints, not the "live" one shooting from the active dot
	void Start () {
		lines = new List<GameObject>();
		//lineRenderer = gameObject.AddComponent<LineRenderer>();
		//lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
		//lineRenderer.widthMultiplier = 0.25f;
	}
	
	public void AddLine(GameObject dot1, GameObject dot2) {
		GameObject newLine = Instantiate(lineRendererPrefab, Vector3.zero, Quaternion.identity);
		// TODO: Figure out a way to avoid GetComponent if possible
		LineRenderer lineRenderer = newLine.GetComponent<LineRenderer>();
		lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
		lineRenderer.widthMultiplier = 0.25f;

		Color lineColor = dot1.gameObject.GetComponent<SpriteRenderer>().color;
		/*Gradient gradient = new Gradient();
		gradient.SetKeys(
			new GradientColorKey[] { new GradientColorKey(lineColor, 0.0f), new GradientColorKey(lineColor, 1.0f) },
			new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
		);*/
		lineRenderer.startColor = lineColor;
		lineRenderer.endColor = lineColor;
		//lineRenderer.colorGradient = gradient;

		Vector3[] points = new Vector3[2];
		points[0] = dot1.transform.position;
		points[1] = dot2.transform.position;

		lineRenderer.SetPositions(points);

		lines.Add(newLine);
	}

	public void RemoveLastLine() {
		Destroy(lines[lines.Count - 1]);
		lines.RemoveAt(lines.Count - 1);
	}

	public void ClearLines() {
		foreach (GameObject line in lines) {
			Destroy(line);
		}
		
		lines.Clear();
	}
	void Update () {
		/*
		List<GameObject> selectedDots = gridManager.GetSelectedDots();

		if (Input.GetMouseButton(0) && selectedDots.Count > 0) {
			lineRenderer.enabled = true;

			// TODO: This obviously can't stick around
			Color lineColor = selectedDots[0].gameObject.GetComponent<SpriteRenderer>().color;
			Gradient gradient = new Gradient();
			gradient.SetKeys(
				new GradientColorKey[] { new GradientColorKey(lineColor, 0.0f), new GradientColorKey(lineColor, 1.0f) },
				new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
			);
			lineRenderer.colorGradient = gradient;
			
			List<Vector3> points = selectedDots.Select((dot) => new Vector3(dot.transform.position.x, dot.transform.position.y, 0.0f)).ToList();
			
			end = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			end.z = 0;

			points.Add(end);

			lineRenderer.positionCount = points.Count;
			lineRenderer.SetPositions(points.ToArray());
		}
		else {
			lineRenderer.enabled = false;
		}
		*/
	}
}
