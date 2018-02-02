using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DumbActor : PongActor {

	public Paddle.Direction MakeDecision(Arena.State state) {
		return ((Time.frameCount / 50) & 1) == 0 ? Paddle.Direction.Left : Paddle.Direction.Right; 
	}

	
	public void OnGameStart(Arena.State state) {}
	public void OnGameEnd(Arena.State state) {}
}
