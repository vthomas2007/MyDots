using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dropper : MonoBehaviour {
	private float startTime;
	private Vector3 startPosition;
	private Vector3 stopPosition;
	
	public float duration = .1f;

	private bool isDropping = false;

	public void Drop(Vector3 startPos, Vector3 stopPos) {
		startTime = Time.time;
		isDropping = true;

		startPosition = startPos;
		gameObject.transform.position = startPosition;

		stopPosition = stopPos;
	}

	void Update() {
		if (isDropping) {
			float t = (Time.time - startTime) / duration;
			
			gameObject.transform.position = new Vector3(
				Mathf.SmoothStep(startPosition.x, stopPosition.x, t),
				Mathf.SmoothStep(startPosition.y, stopPosition.y, t),
				0
			);

			if (t > 1.0f) {
				isDropping = false;
			}
		}
	}
}
