﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActor : PongActor {

	public Paddle.Direction MakeDecision(Arena.State state) {
		return Input.GetKey(KeyCode.A) ? Paddle.Direction.Left : (Input.GetKey(KeyCode.D) ? Paddle.Direction.Right : Paddle.Direction.None);
	}
}