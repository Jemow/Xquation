using UnityEngine;

public class NodeAsin : MathNode
{
    public NodeAsin(MathOp op) : base(op) { }

    public override float Value(float x, float t)
    {
        float clampedX = Mathf.Clamp(x, -1f, 1f);
        return Mathf.Asin(clampedX) + t;
    }

    public override string Formula() => "asin(x)";
}