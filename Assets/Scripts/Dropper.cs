using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; // Exception

public class Dropper : MonoBehaviour {
	public Animator animator;

	public enum AnimationType {
		SmallBounce,
		LargeBounce
	}
	private AnimationType animationType;

	private float startTime;
	private Vector3 startPosition;
	private Vector3 stopPosition;
	
	public float duration = .1f;

	private bool isDropping = false;

	public void Drop(Vector3 startPos, Vector3 stopPos, AnimationType animType) {
		startTime = Time.time;
		isDropping = true;
		animationType = animType;

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

			// TODO
			if (t >= 0.99f) {
				isDropping = false;
				switch (animationType) {
					case AnimationType.SmallBounce:
						animator.SetTrigger("SmallBounce");
						break;
					case AnimationType.LargeBounce:
						animator.SetTrigger("LargeBounce");
						break;
					default:
						throw new Exception("Invalid animation type provided to Drop.");
				}
			}
		}
	}
}
