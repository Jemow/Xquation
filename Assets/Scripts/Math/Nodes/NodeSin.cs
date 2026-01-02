using UnityEngine;

public class NodeSin : MathNode
{
    public NodeSin(MathOp op = MathOp.Multiply) : base(op) {}

    public override float Value(float x) => Mathf.Sin(x);

    public override string Formula() => "sin(x)";
}