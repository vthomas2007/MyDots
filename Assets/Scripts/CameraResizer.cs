﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResizer : MonoBehaviour {
	public float horizontalBuffer = 1.0f;
	public float verticalBuffer = 1.0f;

	// Called "myCamera" to fix Unity warning about overriding Component.camera
	private Camera myCamera;

	public void Start() {
		myCamera = gameObject.GetComponent<Camera>();
	}

	public void Resize(int gridWidth, int gridHeight, float distanceBetweenDots) {
		// Based on the values of the buffers and the aspect ratio, this will only
		// end up honoring one of the two buffers being set
		float contentWidth = gridWidth * distanceBetweenDots + (2.0f * horizontalBuffer);
		float contentHeight = gridHeight * distanceBetweenDots + (2.0f * verticalBuffer);

		float minCameraSize = contentHeight;
		float cameraWidthWithMinCameraSize = minCameraSize * myCamera.aspect;

		if (cameraWidthWithMinCameraSize < contentWidth) {
			float expandWidthAmount = contentWidth - cameraWidthWithMinCameraSize;
			float expandHeightAmount = expandWidthAmount / myCamera.aspect;
			minCameraSize += expandHeightAmount;
		}

		minCameraSize *= 0.5f;

		myCamera.orthographicSize = minCameraSize;
		
		float cameraX = gridWidth * distanceBetweenDots * 0.5f;
		float cameraY = gridHeight * distanceBetweenDots * 0.5f;
		myCamera.gameObject.transform.position = new Vector3(cameraX, cameraY, -1);
	}
}