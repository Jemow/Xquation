using UnityEngine;

public class NodeAbs : MathNode
{
    public NodeAbs(MathOp op) : base(op) { }

    public override float Value(float x) => Mathf.Abs(x);
    public override string Formula() => "abs(x)";
}
