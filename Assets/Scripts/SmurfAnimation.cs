using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmurfAnimation : MonoBehaviour {

    private Vector2 basePosition;

    private void Start() {
        basePosition = transform.position;
    }

    private void Update() {
        transform.position = basePosition + Vector2.down * 0.20f * Mathf.Sin(2.0f * Time.time);
    }
}
