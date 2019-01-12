using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineUpdater : MonoBehaviour {

	private LineRenderer lineRenderer;
	
	void Start () {
		lineRenderer = gameObject.AddComponent<LineRenderer>();
		lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
		lineRenderer.widthMultiplier = 0.5f;

		Gradient gradient = new Gradient();
		gradient.SetKeys(
			new GradientColorKey[] { new GradientColorKey(Color.red, 0.0f), new GradientColorKey(Color.white, 1.0f) },
			new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
		);
		lineRenderer.colorGradient = gradient;
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 start = new Vector2(0.0f,0.0f);
		Vector3 end = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		end.z = 0;
		Vector3[] points = new Vector3[] { start, end };
		lineRenderer.SetPositions(points);
	}
}
