using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SVector2 {

	private float x = 0;
	private float y = 0;

	public float X { get{ return this.x; } }
	public float Y { get{ return this.y; } }

	public SVector2(Vector2 v) {
		this.x = v.x;
		this.y = v.y;
	}
	
	public SVector2(float x, float y) {
		this.x = x;
		this.y = y;
	}

	public Vector2 ToVec2() {
		return new Vector2(x, y);
	}
}
