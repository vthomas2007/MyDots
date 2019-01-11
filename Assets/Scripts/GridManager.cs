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
		
	}
}
