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
	
	public float duration;

	private bool isDropping = false;

	// TODO Rename stuff
	public void Drop(Vector3 startPos, Vector3 stopPos, AnimationType animType, float d = 0.1f, float delay = 0.0f) {
		animationType = animType;
		duration = d;
		animationDelay = delay;

		startPosition = startPos;
		gameObject.transform.position = startPosition;

		stopPosition = stopPos;

		IEnumerator coroutine = EnableIsDropping();
		StartCoroutine(coroutine);
	}
	private IEnumerator EnableIsDropping() {
		yield return new WaitForSeconds(animationDelay);
		isDropping = true;
		startTime = Time.time;
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
			if (t >= 0.98f) {
				isDropping = false;
				gameObject.transform.position = stopPosition;

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

	private IEnumerator DelayForSeconds(float seconds) {
		yield return new WaitForSeconds(seconds);
	}
}
