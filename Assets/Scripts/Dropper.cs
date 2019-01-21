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
	private float animationDelay;
	
	public float dropDuration;

	private bool isDropping = false;

	public void Drop(Vector3 startPos, Vector3 stopPos, AnimationType animType, float duration = 0.1f, float delay = 0.0f) {
		startPosition = startPos;
		stopPosition = stopPos;
		animationType = animType;
		dropDuration = duration;
		animationDelay = delay;

		gameObject.transform.position = startPosition;

		StartCoroutine(EnableDropAfterDelay());
	}
	private IEnumerator EnableDropAfterDelay() {
		yield return new WaitForSeconds(animationDelay);
		isDropping = true;
		startTime = Time.time;
	}

	void Update() {
		if (isDropping) {
			float t = (Time.time - startTime) / dropDuration;
			
			gameObject.transform.position = new Vector3(
				Mathf.SmoothStep(startPosition.x, stopPosition.x, t),
				Mathf.SmoothStep(startPosition.y, stopPosition.y, t),
				0
			);

			if (t > 1.0f) {
				isDropping = false;
				gameObject.transform.position = stopPosition;

				switch (animationType) {
					case AnimationType.SmallBounce:
						animator.Play("SmallBounce");
						break;
					case AnimationType.LargeBounce:
						animator.Play("LargeBounce");
						break;
					default:
						throw new Exception("Invalid animation type provided to Drop.");
				}
			}
		}
	}

	private IEnumerator DelayForSeconds(float seconds) {
		yield return new WaitForSeconds(seconds);
	}
}
