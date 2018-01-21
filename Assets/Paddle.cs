using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour {
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
		Debug.Log(coords);
		float cs = Mathf.Cos(position);
		float sn = Mathf.Sin(position);
		coords = new Vector2(
			cs * coords.x - sn * coords.y,	
			sn * coords.x + cs * coords.y	
		);

		transform.position = new Vector3(coords.x, coords.y, transform.position.z);
	}

	public void UpdatePaddle(Arena.State arenaState, float deltaTime) {
		Direction direction = actor.MakeDecision(arenaState);

		switch (direction)
		{
			case Direction.Left:
				position = Mathf.Max(position + maxSpeed * deltaTime, areaBegin + areaSize);
				break;
			case Direction.Right:
				position = Mathf.Min(position - maxSpeed * deltaTime, areaBegin);
				break;
			default:
				break;
		}

		UpdateWorldPosition();
	}
}
