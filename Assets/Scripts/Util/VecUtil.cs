using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VecUtil {

	public static Vector2 Rotate(Vector2 v, float r) {
		float cs = Mathf.Cos(r);
		float sn = Mathf.Sin(r);
		return new Vector2(
			cs * v.x - sn * v.y,	
			sn * v.x + cs * v.y	
		);
	}
}
