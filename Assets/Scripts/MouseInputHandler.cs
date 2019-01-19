using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInputHandler : MonoBehaviour {
	private GridManager gridManager;

	void Start() {
		gridManager = gameObject.GetComponent<GridManager>();
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
				gridManager.SelectDot(clickedDot);
			}
		}
	}

	private void HandleMouseHold() {
		if (Input.GetMouseButton(0)) {
			gridManager.UpdateLineToCursor();
			
			GameObject dotUnderCursor = GetDotUnderMouseCursor();
			if (dotUnderCursor != null) {
				gridManager.AddOrRemoveDotIfAdjacentToLastSelected(dotUnderCursor);
			}
		}
	}

	private void HandleMouseRelease() {
		if (Input.GetMouseButtonUp(0)) {
			gridManager.RemoveAndDropDots();
		}
	}

	private GameObject GetDotUnderMouseCursor() {
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector2 mousePosition2D = new Vector2(mousePosition.x, mousePosition.y);
		RaycastHit2D hit = Physics2D.Raycast(mousePosition2D, Vector2.zero);

		return (hit.collider == null) ? null : hit.collider.gameObject;
	}
}
