using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
public class Paddle : MonoBehaviour {
	[System.Serializable]
	public enum Direction
	{
		Left,
		Right,
		None,
	}

	// Position is represented along edge of the circular arena is radians
	public float position;
	private float radius;
	private float arenaRadius;
	private float areaBegin;
	private float areaSize;
	private float maxSpeed;

	public float Position{ get{ return position; } }
	public float Radius{ get{ return radius; } }

	private PongActor actor = null;

	public void Initialize(PongActor actor, float radius, float arenaRadius, float areaBegin, float areaSize, float maxSpeed) {
		if(this.actor != null) {
			Debug.LogError("Cannot initialize Paddle twice");
			return;
		}

		this.actor = actor;
		this.radius = radius;
		this.arenaRadius = arenaRadius;
		this.areaBegin = areaBegin;
		this.areaSize = areaSize;
		this.maxSpeed = maxSpeed;

		this.position = areaBegin + areaSize / 2.0f;

		UpdateWorldPosition();
	}

	private void UpdateWorldPosition() {
		transform.rotation = Quaternion.Euler(0, 0, position / (Mathf.PI/180) + 90);
		Vector2 coords = new Vector2(0, arenaRadius + radius / 4f);
		
		coords = VecUtil.Rotate(coords, position);

		transform.position = new Vector3(coords.x, coords.y, transform.position.z);
	}

	public void UpdatePaddle(Arena.State arenaState, float deltaTime) {
		Direction direction = actor.MakeDecision(arenaState);

		switch (direction)
		{
			case Direction.Left:
				position = Mathf.Min(position + maxSpeed * deltaTime, areaBegin + areaSize);
				break;
			case Direction.Right:
				position = Mathf.Max(position - maxSpeed * deltaTime, areaBegin);
				break;
			default:
				break;
		}

		UpdateWorldPosition();
	}

	public static Direction DirectionFromString(string str) {
		if(str.Equals("left")) {
			return Direction.Left;
		} else if(str.Equals("right")) {
			return Direction.Right;
		} else if(str.Equals("none")) {
			return Direction.Left;
		}
		throw new ArgumentException(string.Format("\"{0}\" is not a valid direction", str));
	}
}
