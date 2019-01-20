using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPool : MonoBehaviour {
	public Color[] availableColors;

	public Color GetRandomColor() {
		int colorIndex = UnityEngine.Random.Range(0, availableColors.Length);
		return availableColors[colorIndex];
	}
}