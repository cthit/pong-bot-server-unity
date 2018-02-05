using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[System.Serializable]
public class Ball : MonoBehaviour {
	public SVector2 position = new SVector2(0, 0);
	public SVector2 velocity = new SVector2(0, 0);

	private float speed;
	private float multiplier;

	public void Reset(float speed, float multiplier) {
		this.speed = speed;
		this.multiplier = multiplier;
		
		position = new SVector2(0,0);
		velocity = new SVector2(
			VecUtil.Rotate(
				new Vector2(speed, 0),
				Random.Range(-Mathf.PI, Mathf.PI)
			)
		);
	}

	public Vector2 UpdateBall(Arena.State state, float deltaTime) {

		if(position.ToVec2().magnitude > 4.5) {
			position = new SVector2(0,0);
		}

		position = new SVector2(position.X + velocity.X * deltaTime, position.Y + velocity.Y * deltaTime);
		transform.position = new Vector3(position.X, position.Y, transform.position.z);

		/*List<Tuple<Vector2, float>> collisions = new List<Tuple<Vector2, float>>();
		foreach(Paddle paddle in state.paddles) {
			Tuple<Vector2, float> collision = BallCollision.CheckForCollision(
				position.ToVec2(),
				velocity.ToVec2() * deltaTime,
				new Vector2(paddle.transform.position.x, paddle.transform.position.y),
				transform.lossyScale.x + paddle.Radius
			);
			if(collision != null) {
				collisions.Add(collision);
			}
		}

		if(collisions.Count > 0) {
			Tuple<Vector2, float> closestCollision = collisions[0];
			for(int i = 1; i < collisions.Count; i++) {
				if(collisions[i].y < closestCollision.y) {
					closestCollision = collisions[i];
				}
			}

			Debug.LogFormat("ClosestCollision: {0}", closestCollision.x);
			position = new SVector2(closestCollision.x);
		} else {
			
			Vector2 newPos = position.ToVec2() + velocity.ToVec2();
			Debug.LogFormat("New Pos: {0}   position: {1}   velocity: {2}", newPos, position.ToVec2(), velocity.ToVec2()); 
			position = new SVector2(newPos);
		}


		transform.position = new Vector3(position.X, position.Y, transform.position.z);*/

		return Vector2.zero;
	}

	void OnCollisionEnter2D(Collision2D collision) {
		if(collision.collider.GetType() != typeof(CircleCollider2D)) {
			return;
		}
		CircleCollider2D otherCollider = (CircleCollider2D)collision.collider;
		Transform other = otherCollider.transform;

		Vector2 diff = new Vector2(
			position.X - other.position.x ,
			position.Y - other.position.y 
		);

		float angle = VecUtil.FullAngle(velocity.ToVec2()) - VecUtil.FullAngle(diff);

		Debug.DrawLine(
			position.ToVec2(),
			position.ToVec2() - velocity.ToVec2().normalized * 0.4f,
			Color.red,
			4f
		);

		Debug.DrawLine(
			position.ToVec2(),
			position.ToVec2() + diff.normalized * 0.4f,
			Color.yellow,
			4f
		);

		this.speed *= multiplier;
		velocity = new SVector2(VecUtil.Rotate(-velocity.ToVec2(), -angle * 2).normalized * speed);
		position = new SVector2(
			other.transform.position +
			new Vector3(diff.x, diff.y).normalized *
			(otherCollider.radius + GetComponent<CircleCollider2D>().radius)
		);

		Debug.DrawLine(
			position.ToVec2(),
			position.ToVec2() + velocity.ToVec2().normalized * 0.4f,
			Color.green,
			4f
		);
	}

	public string ToJson() {
		return string.Format("{{\"position\":{0},\"velocity\":{1}}}",
			position.ToJson(),
			velocity.ToJson());
	}
}
