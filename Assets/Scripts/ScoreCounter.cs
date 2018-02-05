using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCounter : MonoBehaviour {

	public TextMesh counterField;
	public TextMesh nameField;

	private Paddle paddle = null;

	public void SetPaddle(Paddle paddle) {
		this.paddle = paddle;
	}

	public void SetName(string name) {
		nameField.text = name;
	}

	void FixedUpdate () {
		if(paddle != null) {
			counterField.text = string.Format("{0}", paddle.Points);
		}
	}
}
