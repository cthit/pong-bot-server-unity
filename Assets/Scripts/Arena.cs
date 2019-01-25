using System.Collections.Generic;
using System.Text;

using UnityEngine;

public class Arena : MonoBehaviour
{

    public float radius = 1.0f;
    public float paddleSpeed = Mathf.PI / 2;
    public float ballSpeed = 2.0f;
    public float ballSpeedMultiplier = 1.03f;
    public int requiredPoints = 10;

    public string[] sampleNames;

    public delegate void OnGameEnd();

    public class State
    {
        public Ball ball;
        public List<Paddle> paddles;
        public OnGameEnd callback;

        public List<Paddle> Paddles { get { return paddles; } }

        public State(Ball ball, List<Paddle> paddles, OnGameEnd callback)
        {
            this.ball = ball;
            this.paddles = paddles;
            this.callback = callback;
        }

        public string ToJson()
        {
            StringBuilder paddleList = new StringBuilder();

            for (int i = 0; i < paddles.Count; i++)
            {
                paddleList.Append(paddles[i].ToJson());
                if (i < paddles.Count - 1)
                {
                    paddleList.Append(',');
                }
            }

            return string.Format("{{\"ball\":{0}, \"paddles\":[{1}]}}",
                ball.ToJson(),
                paddleList.ToString()
            );
        }
    }

    public GameObject ballPrefab;
    public GameObject paddlePrefab;
    public GameObject scoreCounterPrefab;

    private State state = null;
    public bool GameStarted { get { return state != null; } }

    public void StartGame(List<PongActor> actors, OnGameEnd callback)
    {
        if (GameStarted)
        {
            Debug.LogError("Game Already Started");
            return;
        }

        if (actors.Count == 0)
        {
            Debug.LogError("Can't start a game with 0 players");
            return;
        }

        List<Paddle> paddles = new List<Paddle>();

        float areaSize = Mathf.PI * 2f / actors.Count;
        float paddleRadius = 0.4f;//1f - 0.2f * actors.Count;

        for (int i = 0; i < actors.Count; i++)
        {
            GameObject paddleObject = GameObject.Instantiate(paddlePrefab, transform);
            Paddle paddle = paddleObject.AddComponent<Paddle>();
            paddle.Initialize(actors[i], paddleRadius, radius, areaSize * i, areaSize, paddleSpeed);
            paddles.Add(paddle);
            paddleObject.name = string.Format("Player {0}", i);

            Vector2 spos = VecUtil.Rotate(new Vector2(0, -radius * 1.3f), areaSize * i + areaSize / 2f);
            GameObject scoreCounterObject = GameObject.Instantiate(
                scoreCounterPrefab,
                new Vector3(spos.x, spos.y, 0),
                Quaternion.identity,
                transform
            );
            scoreCounterObject.name = string.Format("Player {0} Score Counter", i);
            ScoreCounter sc = scoreCounterObject.GetComponent<ScoreCounter>();
            sc.SetPaddle(paddle);
            if (actors[i] is Client)
            {
                sc.SetName(((Client)actors[i]).info.name);
            }
            else
            {
                sc.SetName(sampleNames[Random.Range(0, sampleNames.Length)]);
            }
        }

        GameObject ballObject = GameObject.Instantiate(ballPrefab, Vector3.zero, Quaternion.identity, transform);
        Ball ball = ballObject.AddComponent<Ball>();
        ball.Reset(ballSpeed, ballSpeedMultiplier);

        state = new State(ball, paddles, callback);

        foreach (PongActor actor in actors)
        {
            actor.OnGameStart(state);
        }
    }

    public void StopGame()
    {
        if (!GameStarted)
        {
            Debug.LogError("Game Already Stopped");
            return;
        }

        foreach (Paddle paddle in state.Paddles)
        {
            paddle.Actor.OnGameEnd(state);
        }

        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }

        OnGameEnd callback = state.callback;

        state = null;

        if (callback != null)
        {
            callback();
        }
    }

    private void DescorePaddle(Paddle scoree)
    {
        List<Paddle> paddles = state.paddles;
        foreach (Paddle paddle in paddles)
        {
            if (scoree == paddle) continue;

            paddle.IncrementPoints();
        }

        int winner = -1;
        for (int i = 0; i < paddles.Count; i++)
        {
            if (paddles[i].Points >= requiredPoints)
            {
                if (winner >= 0)
                {
                    if (paddles[winner].Points > paddles[i].Points)
                    {
                        continue;
                    }
                    else if (paddles[winner].Points == paddles[i].Points)
                    {
                        winner = -1;
                        break;
                    }
                }
                winner = i;
            }
        }

        if (winner >= 0)
        {
            Debug.LogFormat("Player {0} won!", winner);
            if (paddles[winner].Actor is Client)
            {
                Notifier.Notify(string.Format("{0} has won!", ((Client)paddles[winner].Actor).info.name), 7f);
            }
            else
            {
                Notifier.Notify(string.Format("A bot won the game..."), 5f);
            }
            StopGame();
        }
    }

    void FixedUpdate()
    {
        if (GameStarted)
        {
            foreach (Paddle paddle in state.Paddles)
            {
                paddle.UpdatePaddle(state, Time.fixedDeltaTime);
            }
            state.ball.UpdateBall(state, Time.fixedDeltaTime);

            if (state.ball.position.ToVec2().magnitude > radius)
            {
                float angle = VecUtil.FullAngle(state.ball.position.ToVec2());

                GameObject.Destroy(state.ball.gameObject);
                GameObject ballObject = GameObject.Instantiate(ballPrefab, Vector3.zero, Quaternion.identity, transform);
                state.ball = ballObject.AddComponent<Ball>();
                state.ball.Reset(ballSpeed, ballSpeedMultiplier);

                for (int i = 0; i < state.Paddles.Count; i++)
                {
                    if (angle < state.Paddles[i].AreaEnd)
                    {
                        //Debug.LogFormat("Player {0} lost a point!", i);
                        DescorePaddle(state.Paddles[i]);
                        break;
                    }
                }
            }
        }
    }

    void Start()
    {
#if false // Debug setup
        List<PongActor> actors = new List<PongActor>();
        actors.Add(new PlayerActor());
        actors.Add(new PlayerActor());
        actors.Add(new PlayerActor());
        actors.Add(new PlayerActor());
        actors.Add(new PlayerActor());
        actors.Add(new PlayerActor());
        actors.Add(new PlayerActor());
        actors.Add(new PlayerActor());
        actors.Add(new PlayerActor());
        actors.Add(new PlayerActor());
        actors.Add(new PlayerActor());
        actors.Add(new PlayerActor());
        actors.Add(new PlayerActor());
        actors.Add(new PlayerActor());
        StartGame(actors, null);
#endif
    }
}
