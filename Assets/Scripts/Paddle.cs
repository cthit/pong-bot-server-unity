using System;
using System.Globalization;

using UnityEngine;

[System.Serializable]
public class Paddle : MonoBehaviour
{
    [System.Serializable]
    public enum Direction
    {
        Left,
        Right,
        None,
    }

    // Position is represented along edge of the circular arena is radians
    public float position;
    private float radius;
    private float arenaRadius;
    private float areaBegin;
    private float areaSize;
    private float maxSpeed;
    private int points = 0;
    private PongActor actor = null;

    private Direction latestDecision;


    public float Position { get { return position; } }
    public float Radius { get { return radius; } }
    public float AreaBegin { get { return areaBegin; } }
    public float AreaSize { get { return areaSize; } }
    public float AreaEnd { get { return areaBegin + areaSize; } }
    public int Points { get { return points; } }
    public PongActor Actor { get { return actor; } }

    public void Initialize(PongActor actor, float radius, float arenaRadius, float areaBegin, float areaSize, float maxSpeed)
    {
        if (this.actor != null)
        {
            Debug.LogError("Cannot initialize Paddle twice");
            return;
        }

        this.actor = actor;
        this.radius = radius;
        this.arenaRadius = arenaRadius;
        this.areaBegin = areaBegin;
        this.areaSize = areaSize;
        this.maxSpeed = maxSpeed;

        this.position = areaBegin + areaSize / 2.0f;

        UpdateWorldPosition();
    }

    public int IncrementPoints()
    {
        return ++points;
    }

    private void UpdateWorldPosition()
    {
        transform.rotation = Quaternion.Euler(0, 0, position / (Mathf.PI / 180f) - 90f);
        Vector2 coords = new Vector2(0, -(arenaRadius + radius / 2f));

        coords = VecUtil.Rotate(coords, position);

        transform.position = new Vector3(coords.x, coords.y, transform.position.z);
    }

    public void UpdatePaddle(Arena.State arenaState, float deltaTime)
    {
        latestDecision = actor.MakeDecision(arenaState);

        switch (latestDecision)
        {
            case Direction.Left:
                position = Mathf.Max(position - maxSpeed * deltaTime, areaBegin);
                break;
            case Direction.Right:
                position = Mathf.Min(position + maxSpeed * deltaTime, areaBegin + areaSize);
                break;
            default:
                break;
        }

        UpdateWorldPosition();
    }

    public static Direction DirectionFromString(string str)
    {
        Debug.Log("Recieved paddle direction: " + str);
        string lower = str.ToLower();
        if (lower.Equals("move_clockwise"))
        {
            return Direction.Left;
        }
        else if (lower.Equals("move_counterclockwise"))
        {
            return Direction.Right;
        }
        else if (lower.Equals("stop"))
        {
            return Direction.None;
        }
        throw new ArgumentException(string.Format("\"{0}\" is not a valid direction", str));
    }

    public string ToJson()
    {
        return string.Format(new CultureInfo("en-US"), "{{\"player\":{0},\"angle\":{1},\"state\":\"{2}\"}}",
            actor is Client ? ((Client)actor).ID : 0,
            position - (Mathf.PI / 2),
            latestDecision == Direction.Left ? "moving_clockwise" : (latestDecision == Direction.Right ? "moving_counterclockwise" : "stop"));
    }
}
