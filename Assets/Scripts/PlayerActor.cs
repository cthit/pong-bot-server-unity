using UnityEngine;

public class PlayerActor : PongActor
{

    public Paddle.Direction MakeDecision(Arena.State state)
    {
        return Input.GetKey(KeyCode.A) ? Paddle.Direction.Left :
              (Input.GetKey(KeyCode.D) ? Paddle.Direction.Right : Paddle.Direction.None);
    }

    public void OnGameStart(Arena.State state) { }
    public void OnGameEnd(Arena.State state) { }
}
