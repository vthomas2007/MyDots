using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour {
	private static int WIDTH = 6;
	private static int HEIGHT = 6;
	private GameObject[,] dots = new GameObject[WIDTH, HEIGHT];

	public GameObject dotPrefab;

	public float dotScale = 1.0f;
	public float distanceBetweenDots = 1.0f;

	private List<GameObject> activeDots = new List<GameObject>();

	// Use this for initialization
	void Start () {
		for (int i = 0; i < HEIGHT; i++) {
			for (int j = 0; j < WIDTH; j++) {
				dots[i,j] = Instantiate(dotPrefab, new Vector3((float)i * distanceBetweenDots, (float)j * distanceBetweenDots), Quaternion.identity);
				dots[i,j].transform.localScale = new Vector2(dotScale, dotScale);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		HandleMouseClick();
		HandleMouseHold();
		HandleMouseRelease();
	}

	public GameObject GetCurrentDot() {
		if (activeDots.Count > 0) {
			return activeDots[activeDots.Count - 1];
		}
		return null;
	}
	// TODO: See if this can easily be moved to another class or even gameObject
	private void HandleMouseClick() {
		if (Input.GetMouseButtonDown(0)) {
			Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			Vector2 mousePosition2D = new Vector2(mousePosition.x, mousePosition.y);
			RaycastHit2D hit = Physics2D.Raycast(mousePosition2D, Vector2.zero);

			if (hit.collider != null) {
				activeDots.Add(hit.collider.gameObject);
			}
		}
	}

	private void HandleMouseHold() {
		// TODO: Right now this is being handled in the line updater.
		// Either move that into this class or move all the input handling
		// into another class and find a clean way to pass information betwee
		// all classes
	}

	private void HandleMouseRelease() {
		if (Input.GetMouseButtonUp(0)) {
			activeDots.Clear();
		}
	}
}
