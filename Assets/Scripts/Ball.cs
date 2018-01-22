using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[System.Serializable]
public class Ball : MonoBehaviour {
	public SVector2 position = new SVector2(0, 0);
	public SVector2 velocity = new SVector2(0, 0);

	public void Reset(float speed) {
		position = new SVector2(0,0);
		velocity = new SVector2(
			VecUtil.Rotate(
				new Vector2(speed, 0),
				Random.Range(-Mathf.PI, Mathf.PI)
			)
		);
	}

	public void UpdateBall(Arena.State state, float deltaTime) {

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
	}

	void OnCollisionEnter2D(Collision2D collision) {
		Collider2D otherCollider = collision.otherCollider;
		if(otherCollider.GetType() != typeof(CircleCollider2D)) {
			return;
		}

		Transform other = otherCollider.transform;
		Vector2 diff = new Vector2(
			other.position.x - position.X,
			other.position.y - position.Y
		);
		float angle = Vector2.Angle(velocity.ToVec2(), diff);

		velocity = new SVector2(VecUtil.Rotate(velocity.ToVec2(), angle * 2));
		position = new SVector2(
			other.transform.position +
			new Vector3(diff.x, diff.y).normalized *
			(((CircleCollider2D)otherCollider).radius + GetComponent<CircleCollider2D>().radius)
		);
	}
}
