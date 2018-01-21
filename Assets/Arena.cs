using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arena : MonoBehaviour {

	public float radius = 1.0f;
	public float paddleSpeed = Mathf.PI;

	public class State {
		List<Paddle> paddles;

		public List<Paddle> Paddles { get{ return paddles; } }

		public State(List<Paddle> paddles) {
			this.paddles = paddles;
		}
	}

	public GameObject ballPrefab;
	public GameObject paddlePrefab;

	private State state = null;
	
	public void StartGame(List<PongActor> actors) {
		List<Paddle> paddles = new List<Paddle>();

		float areaSize = Mathf.PI * 2f / actors.Count;
		float paddleRadius = 1f - 0.2f * actors.Count;
		
		for(int i = 0; i < actors.Count; i++) {
			GameObject paddleObject = GameObject.Instantiate(paddlePrefab);
			Paddle paddle = paddleObject.AddComponent<Paddle>();
			paddle.Initialize(actors[i], paddleRadius, radius, areaSize * i, areaSize, paddleSpeed);
			paddles.Add(paddle);
		}

		state = new State(paddles);
	}

	void FixedUpdate() {
		if(state != null) {
			foreach(Paddle paddle in state.Paddles) {
				paddle.UpdatePaddle(state, Time.fixedDeltaTime);
			}
		}
	}

	void Start() {
		List<PongActor> actors = new List<PongActor>();
		actors.Add(new DumbActor());
		actors.Add(new DumbActor());
		actors.Add(new DumbActor());
		StartGame(actors);
	}
}
