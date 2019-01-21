using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInputHandler : MonoBehaviour {
	private DotsGameManager gameManager;

	void Start() {
		gameManager = gameObject.GetComponent<DotsGameManager>();
	}
	
	void Update() {
		HandleMouseClick();
		HandleMouseHold();
		HandleMouseRelease();
	}

	private void HandleMouseClick() {
		if (Input.GetMouseButtonDown(0)) {
			GameObject clickedDot = GetDotUnderMouseCursor();

			if (clickedDot != null) {
				gameManager.SelectDot(clickedDot);
			}
		}
	}

	private void HandleMouseHold() {
		if (Input.GetMouseButton(0)) {
			gameManager.UpdateLineToCursor();
			
			GameObject dotUnderCursor = GetDotUnderMouseCursor();
			if (dotUnderCursor != null) {
				gameManager.AddToOrRemoveFromSelectedList(dotUnderCursor);
			}
		}
	}

	private void HandleMouseRelease() {
		if (Input.GetMouseButtonUp(0)) {
			gameManager.RemoveAndDropDots();
		}
	}

	private GameObject GetDotUnderMouseCursor() {
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector2 mousePosition2D = new Vector2(mousePosition.x, mousePosition.y);
		RaycastHit2D hit = Physics2D.Raycast(mousePosition2D, Vector2.zero);

		return (hit.collider == null) ? null : hit.collider.gameObject;
	}
}