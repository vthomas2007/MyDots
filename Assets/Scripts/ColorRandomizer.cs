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

	public bool easyMode = false;
	
	// TODO: Determine where this should live, putting it on the dot prevents
	// the manager from having any say over the assigned color, how many colors
	// to choose from, etc
	void OnEnable() {
		int colorIndex = !easyMode ? Random.Range(0, colors.Length) : Random.Range(0, 2);;
		spriteRenderer.color = colors[colorIndex];
	}
}
