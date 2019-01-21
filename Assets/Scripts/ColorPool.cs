using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPool : MonoBehaviour {
	[SerializeField]
	private Color[] availableColors;

	public Color this[int i] {
		get { return availableColors[i]; }
	}

	public Color GetRandomColor() {
		int colorIndex = UnityEngine.Random.Range(0, availableColors.Length);
		return availableColors[colorIndex];
	}

	public int Count() {
		return availableColors.Length;
	}
}