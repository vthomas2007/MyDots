using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotColorUpdater : MonoBehaviour {
	public SpriteRenderer spriteRenderer;

	public void UpdateColor(Color c) {
		spriteRenderer.color = c;
	}
}
