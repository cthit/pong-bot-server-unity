public interface PongActor
{
    Paddle.Direction MakeDecision(Arena.State state);
    void OnGameStart(Arena.State state);
    void OnGameEnd(Arena.State state);
}
