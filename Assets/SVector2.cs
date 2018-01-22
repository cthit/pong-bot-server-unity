using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SVector2 {

	public float x;
	public float y;

	public SVector2(Vector2 v) {
		Set(v);
	}
	
	public SVector2(float x, float y) {
		Set(x, y);
	}

	public Vector2 ToVec2() {
		return new Vector2(x, y);
	}

	public void Set(Vector2 v) {
		this.x = v.x;
		this.y = v.y;
	}

	public void Set(float x, float y) {
		this.x = x;
		this.y = y;
	}
}
