using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class Arena : MonoBehaviour {

	public float radius = 1.0f;
	public float paddleSpeed = Mathf.PI / 2;
	public float ballSpeed = 2.0f;

	public delegate void OnGameEnd();

	public class State {
		public Ball ball;
		public List<Paddle> paddles;
		public OnGameEnd callback;

		public List<Paddle> Paddles { get{ return paddles; } }

		public State(Ball ball, List<Paddle> paddles, OnGameEnd callback) {
			this.ball = ball;
			this.paddles = paddles;
			this.callback = callback;
		}

		public string ToJson() {
			StringBuilder paddleList = new StringBuilder();

			for(int i = 0; i < paddles.Count; i++) {
				paddleList.Append(JsonUtility.ToJson(paddles[i]));
				if(i < paddles.Count - 1) {
					paddleList.Append(',');
				}
			}

			return string.Format("{{\"ball\":{0}, \"paddles\":[{1}]}}",
				JsonUtility.ToJson(ball),
				paddleList.ToString()
			);
		}
	}

	public GameObject ballPrefab;
	public GameObject paddlePrefab;

	private State state = null;
	
	public void StartGame(List<PongActor> actors, OnGameEnd callback) {
		if(state != null) {
			Debug.LogError("Game Already Started");
			return;
		}

		if(actors.Count == 0) {
			Debug.LogError("Can't start a game with 0 players");
			return;
		}

		List<Paddle> paddles = new List<Paddle>();

		float areaSize = Mathf.PI * 2f / actors.Count;
		float paddleRadius = 1f - 0.2f * actors.Count;
		
		for(int i = 0; i < actors.Count; i++) {
			GameObject paddleObject = GameObject.Instantiate(paddlePrefab, transform);
			Paddle paddle = paddleObject.AddComponent<Paddle>();
			paddle.Initialize(actors[i], paddleRadius, radius, areaSize * i, areaSize, paddleSpeed);
			paddles.Add(paddle);
		}

		GameObject ballObject = GameObject.Instantiate(ballPrefab, Vector3.zero, Quaternion.identity, transform);
		Ball ball = ballObject.AddComponent<Ball>();
		ball.Reset(ballSpeed);

		state = new State(ball, paddles, callback);
	}

	public void StopGame() {
		if(state == null) {
			Debug.LogError("Game Already Stopped");
			return;
		}

		foreach(Transform child in transform) {
			GameObject.Destroy(child.gameObject);
		}

		if(state.callback != null) {
			state.callback();
		}

		state = null;
	}

	void FixedUpdate() {
		if(state != null) {
			foreach(Paddle paddle in state.Paddles) {
				paddle.UpdatePaddle(state, Time.fixedDeltaTime);
			}
			state.ball.UpdateBall(state, Time.fixedDeltaTime);

			if(state.ball.position.ToVec2().magnitude > radius) {
				state.ball.Reset(ballSpeed);
			}
		}
	}

	void Start() {
		List<PongActor> actors = new List<PongActor>();
		actors.Add(new PlayerActor());
		actors.Add(new DumbActor());
		//StartGame(actors);
	}
}
