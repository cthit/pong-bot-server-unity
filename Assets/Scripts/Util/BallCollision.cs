using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BallCollision {

	// Returns point of collision
	public static Tuple<Vector2, float> CheckForCollision(Vector2 pointA, Vector2 velocity, Vector2 pointB, float combinedRadii) {
		
		Debug.LogFormat("{0}.magnitude == {1}", velocity, velocity.magnitude);
		if(velocity.magnitude == 0) {
			return null;
		}
		
		Vector2 diff = pointA - pointB;

		if(diff.magnitude == 0) {
			return null;
		}

		float angle = (velocity.x * diff.x + velocity.y * diff.y) / (velocity.magnitude * diff.magnitude);
		Debug.Log(angle);

		float distToClosestPoint = Mathf.Cos(angle) * diff.magnitude;
		float distAtClosestPoint = Mathf.Sin(angle) * diff.magnitude;

		if (distAtClosestPoint >= combinedRadii) {
			// No collision will occur
			return null;
		}

		float distToCollision = distToClosestPoint - 
			Mathf.Sin(Mathf.Acos(distAtClosestPoint / combinedRadii)) *
			combinedRadii;

		if(distToCollision == float.NaN) {
			return null;
		}

		if(distToCollision > velocity.magnitude) {
			return null;
		}

		Vector2 collisionPoint = velocity.normalized * distToCollision;

		return new Tuple<Vector2, float>(collisionPoint, distToCollision);
	}
}
