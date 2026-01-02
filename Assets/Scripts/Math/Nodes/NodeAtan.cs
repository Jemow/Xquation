using UnityEngine;

public class NodeAtan : MathNode
{
    public NodeAtan(MathOp op) : base(op) { }

    public override float Value(float x) => Mathf.Atan(x);

    public override string Formula() => "atan(x)";
}
