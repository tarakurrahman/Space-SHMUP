using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// To type the next 4 lines, start by typing /// and then Tab.
/// <summary>
/// Keeps a GameObject on screen.
/// Note that this ONLY works for an orthographic Main Camera at [ 0, 0, 0 ].
/// </summary>
public class BoundsCheck : MonoBehaviour {
	[Header("Set in Inspector")]
	public float radius = 1f;
	public bool keepOnScreen = true;

	[Header("Set Dynamically")]
	public bool isOnScreen = true;
	public float camWidth;
	public float camHeight;

	void Awake() {
		camHeight = Camera.main.orthographicSize;
		camWidth = camHeight * Camera.main.aspect;
	}

	void LateUpdate () {
		Vector3 pos = transform.position; // c
		isOnScreen = true; // d

		if ( pos.x > camWidth - radius ) {
			pos.x = camWidth - radius;
			isOnScreen = false; // e
		}

		if ( pos.x < -camWidth + radius ) {
			pos.x = -camWidth + radius;
			isOnScreen = false; // e
		}

		if ( pos.y > camHeight - radius ) {
			pos.y = camHeight - radius;
			isOnScreen = false; // e
		}

		if ( pos.y < -camHeight + radius ) {
			pos.y = -camHeight + radius;
			isOnScreen = false; // e
		}

		if ( keepOnScreen && !isOnScreen ) { // f
			transform.position = pos; // g
			isOnScreen = true;
		}
	}

	void OnDrawGizmos () {
		if (!Application.isPlaying)
			return;
		Vector3 boundSize = new Vector3(camWidth * 2, camHeight * 2, 0.1f);
		Gizmos.DrawWireCube(Vector3.zero, boundSize);
	}
}