using UnityEngine;

public class VecUtil
{

    public static Vector2 Rotate(Vector2 v, float r)
    {
        float cs = Mathf.Cos(r);
        float sn = Mathf.Sin(r);
        return new Vector2(
            cs * v.x - sn * v.y,
            sn * v.x + cs * v.y
        );
    }

    public static float FullAngle(Vector2 v)
    {
        float angle = Vector2.SignedAngle(Vector2.down, v);
        angle = angle >= 0 ? angle : 360f + angle;
        return angle * Mathf.PI / 180f;
    }
}
