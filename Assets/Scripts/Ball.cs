using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ball : MonoBehaviour {
	public SVector2 position;
	public SVector2 velocity;

	void Start () {
		position = new SVector2(0, 0);
		velocity = new SVector2(0, 0);
	}
}
