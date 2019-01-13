using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorRandomizer {
	public SpriteRenderer spriteRenderer;

	private static Color YELLOW = new Color32(230, 219, 33,  255);
	private static Color GREEN  = new Color32(140, 235, 148, 255);
	private static Color BLUE   = new Color32(140, 190, 255, 255);
	private static Color RED    = new Color32(239, 93,  66,  255);
	private static Color PURPLE = new Color32(156, 93,  181, 255);

	public static Color[] colors = { YELLOW, GREEN, BLUE, RED, PURPLE };
	
	public static Color GetRandomColor() {
		int colorIndex = Random.Range(0, colors.Length);
		return colors[colorIndex];
		//spriteRenderer.color = colors[colorIndex];
	}
}
