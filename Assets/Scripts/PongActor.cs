using System.Collections;
using System.Collections.Generic;

public interface PongActor {
	Paddle.Direction MakeDecision(Arena.State state);
	void OnGameStart(Arena.State state);	
	void OnGameEnd(Arena.State state);
}
