using System.Collections;
using System.Collections.Generic;

public interface PongActor {
	Paddle.Direction MakeDecision(Arena.State state);
}
