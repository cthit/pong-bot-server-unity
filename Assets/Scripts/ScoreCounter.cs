using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCounter : MonoBehaviour {

	public TextMesh counterField;

	private Paddle paddle = null;

	public void SetPaddle(Paddle paddle) {
		this.paddle = paddle;
	}

	void FixedUpdate () {
		if(paddle != null) {
			counterField.text = string.Format("{0}", paddle.Points);
		}
	}
}
