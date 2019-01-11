using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorRandomizer : MonoBehaviour {
	public SpriteRenderer spriteRenderer;

	static private Color YELLOW = new Color32(230, 219, 33,  255);
	static private Color GREEN  = new Color32(140, 235, 148, 255);
	static private Color BLUE   = new Color32(140, 190, 255, 255);
	static private Color RED    = new Color32(239, 93,  66,  255);
	static private Color PURPLE = new Color32(156, 93,  181, 255);

	static public Color[] colors = { YELLOW, GREEN, BLUE, RED, PURPLE };
	
	void Start () {
		int colorIndex = Random.Range(0, colors.Length);
		spriteRenderer.color = colors[colorIndex];
	}
}
