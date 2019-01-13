using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LineUpdater : MonoBehaviour {

	private LineRenderer lineRenderer;
	public GridManager gridManager;

	private GameObject currentDot;
	private Vector3 start;
	private Vector3 end;
	
	// TODO TODO TODO: Flip the dependency between the line renderer and the grid
	// If the grid knows about the line renderer, it can just update/notify it when things change
	// Alternative: Use events. But may not surface enough information about activedots etc.
	void Start () {
		lineRenderer = gameObject.AddComponent<LineRenderer>();
		lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
		lineRenderer.widthMultiplier = 0.25f;

		Gradient gradient = new Gradient();
		gradient.SetKeys(
			new GradientColorKey[] { new GradientColorKey(Color.red, 0.0f), new GradientColorKey(Color.white, 1.0f) },
			new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
		);
		lineRenderer.colorGradient = gradient;
	}
	
	void Update () {
		List<GameObject> activeDots = gridManager.GetActiveDots();

		if (Input.GetMouseButton(0) && activeDots.Count > 0) {
			lineRenderer.enabled = true;

			// TODO: This obviously can't stick around
			Color lineColor = activeDots[0].gameObject.GetComponent<SpriteRenderer>().color;
			Gradient gradient = new Gradient();
			gradient.SetKeys(
				new GradientColorKey[] { new GradientColorKey(lineColor, 0.0f), new GradientColorKey(lineColor, 1.0f) },
				new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
			);
			lineRenderer.colorGradient = gradient;
			
			List<Vector3> points = activeDots.Select((dot) => new Vector3(dot.transform.position.x, dot.transform.position.y, 0.0f)).ToList();
			
			end = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			end.z = 0;

			points.Add(end);

			Debug.Log(points.Count);
			lineRenderer.positionCount = points.Count;
			lineRenderer.SetPositions(points.ToArray());
		}
		else {
			lineRenderer.enabled = false;
		}
	}
}
