using UnityEngine;

public class NodeSin : MathNode
{
    public NodeSin(MathOp op = MathOp.Multiply) : base(op) {}

    public override float Value(float x, float t) => Mathf.Sin(x + t);

    public override string Formula() => "sin(x)";
}